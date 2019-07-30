using System;
using System.Linq;
using System.Text;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;
using Newtonsoft.Json;
using MarioMaker2OCR.Objects;
namespace MarioMaker2OCR
{
    internal static class SMMServer
    {
        private static WebServer server;
        private static SMMWebSocketServer wss;
        public static ushort port = 3001;
        public static string webPath = "./web/";

        public static LevelWrapper LastLevelTransmitted { get; private set; }


        /// <summary>
        /// Starts the primary Web server and Web Socket Server
        /// </summary>
        public static void Start()
        {
            if (server != null) return;
            server = new WebServer(String.Format("http://localhost:{0}/", port));

            // FIXME: EmbedIO 3.0 is moving to a more general FileModule in-place of this one
            server.RegisterModule(new StaticFilesModule(webPath));

            wss = new SMMWebSocketServer();
            server.RegisterModule(new WebSocketsModule());
            server.Module<WebSocketsModule>().RegisterWebSocketsServer<SMMWebSocketServer>("/wss", wss);

            server.RunAsync();
        }

        /// <summary>
        /// Stops and Disposes of the Web and Web Socket Servers
        /// </summary>
        public static void Stop()
        {
            if (server == null) return;

            wss.Dispose();
            server.Dispose();
            server = null;
            wss = null;
        }

        /// <summary>
        /// Publishes the event to all connected web sockets.
        /// </summary>
        /// <param name="eventType">Event name</param>
        public static void BroadcastEvent(String eventType)
        {
            string json = JsonConvert.SerializeObject(new EventWrapper() { type = eventType });
            Broadcast(json);

            // last level is invalid if exitted.
            if (eventType == "exit" || eventType == "gameover" || eventType == "next") LastLevelTransmitted = null;
        }

        /// <summary>
        /// Publishes a new level to all connected web sockets.
        /// </summary>
        /// <param name="newLevel">Level information to be published</param>
        public static void BroadcastLevel(Level newLevel)
        {
            LevelWrapper levelWrapper = new LevelWrapper() { level = newLevel };
            string json = JsonConvert.SerializeObject(levelWrapper);
            Broadcast(json);

            // track last level submitted
            LastLevelTransmitted = levelWrapper;
        }
        public static void BroadcastDataEvent(string eventType, string data)
        {
            string json = JsonConvert.SerializeObject(new DataEventWrapper() { type = eventType, data = data });
            Broadcast(json);
        }
        public static void Broadcast(string message)
        {
            foreach (var ws in wss.WebSockets)
            {
                ws.WebSocket.SendAsync(Encoding.UTF8.GetBytes(message), true);
            }
        }
    }

    internal class SMMWebSocketServer : WebSocketsServer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SMMWebSocketServer() : base(true)
        {
        }
        public override string ServerName => "SMM WebSocketServer";

        protected override void OnMessageReceived(IWebSocketContext context, byte[] rxBuffer, IWebSocketReceiveResult rxResult)
        {
            //Effectively an echo server, anything one client sends gets sent to all connections. This allows other 'bots' to potentially interact and publish their own events.
            foreach (var ws in WebSockets)
            {
                if (ws != context) Send(ws, rxBuffer.ToArray());
            }
        }

        protected override void OnClientConnected(IWebSocketContext context, System.Net.IPEndPoint localEndPoint, System.Net.IPEndPoint remoteEndPoint)
        {
            // Send the cached level on connect - if available
            if (SMMServer.LastLevelTransmitted != null)
            {
                string message = JsonConvert.SerializeObject(SMMServer.LastLevelTransmitted);
                Send(context, message);
            }
        }

        protected override void OnFrameReceived(IWebSocketContext context, byte[] rxBuffer, IWebSocketReceiveResult rxResult)
        {
            // Do Nothing
        }

        protected override void OnClientDisconnected(IWebSocketContext context)
        {
            // Do Nothing
        }
    }
}

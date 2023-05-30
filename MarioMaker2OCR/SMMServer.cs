﻿using System;
using System.Linq;
using System.Text;
using EmbedIO;
using EmbedIO.WebSockets;
using Newtonsoft.Json;
using MarioMaker2OCR.Objects;
using EmbedIO.Files;
using System.Threading.Tasks;
using DirectShowLib;

namespace MarioMaker2OCR
{
    internal static class SMMServer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            wss = new SMMWebSocketServer();
            server = new WebServer(o => o
                .WithUrlPrefix($"http://localhost:{port}/")
                .WithMode(HttpListenerMode.EmbedIO))
                .WithModule(wss)
                .WithStaticFolder("/", webPath, true);

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
            try
            {
                wss.Broadcast(message);
            }
            catch (Exception e)
            {
                log.Error("Exception in WebSocket Broadcast() - " + e.Message);
            }
        }
    }

    internal class SMMWebSocketServer : WebSocketModule
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public SMMWebSocketServer() : base("/wss", true)
        {

        }

        public void Broadcast(string message)
        {
            BroadcastAsync(message);
        }

        protected override async Task OnMessageReceivedAsync(IWebSocketContext context, byte[] rxBuffer, IWebSocketReceiveResult rxResult)
        {

            //Effectively an echo server, anything one client sends gets sent to all connections. This allows other 'bots' to potentially interact and publish their own events.
            await BroadcastAsync(Encoding.UTF8.GetString(rxBuffer));
            
        }

        protected override async Task OnClientConnectedAsync(IWebSocketContext context)
        {
            // Send the cached level on connect - if available
            if (SMMServer.LastLevelTransmitted != null)
            {
                string message = JsonConvert.SerializeObject(SMMServer.LastLevelTransmitted);
                await SendAsync(context, message);
            }
        }

        protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            return Task.CompletedTask;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace MarioMaker2OCR
{
    class WarpWorldAPI
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string username;
        private string token;
        private string base_url = "https://api.warp.world";
        private int queue;

        public WarpWorldAPI(string username, string token)
        {
            this.username = username;
            this.token = token;
            queue = getQueueId();
            if(queue <= 0)
            {
                throw new Exception("Unable to find an active Warp.World queue for the current user");
            }
    
        }

        /// <summary>
        /// Create a new HttpWebRequest object with appropiate settings for communication with Warp.World
        /// </summary>
        /// <param name="method">HTTP Method for the request object</param>
        /// <param name="url">URL the request will be made to</param>
        /// <returns>An initialized HttpWebRequest object</returns>
        private HttpWebRequest newRequestObject(string method, string url)
        {
            HttpWebRequest obj = (HttpWebRequest)WebRequest.Create(url);
            obj.Method = method.ToUpper();
            obj.Timeout = 10000;
            obj.UserAgent = getUserAgent();
            return obj;
        }

        /// <summary>
        /// Generates the userAgent the application should use, includes the application version and a contact email
        /// </summary>
        /// <returns>The application's UserAgent</returns>
        public static string getUserAgent()
        {
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            return "MarioMaker2OCR/" + fileVersionInfo.ProductVersion + " (zi@0x0539.net)";
        }

        /// <summary>
        /// Performas a GET request against api.warp.world/{username}/{enpoint}
        /// </summary>
        /// <param name="endpoint">The endpoint to hit</param>
        /// <returns>The parsed response body, or a body with res.success=false</returns>
        public dynamic _get(string endpoint)
        {
            var req = newRequestObject("GET", String.Format("{0}/{1}/{2}", base_url, username, endpoint));
            try
            {
                var res = new StreamReader(req.GetResponse().GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.Linq.JObject.Parse(res);
            }
            catch (Exception ex)
            {
                log.Error("Error while connecting to warp world.", ex);
            }
            return Newtonsoft.Json.Linq.JObject.Parse("{\"success\":false, \"message\":\"Error connecting to warp world.\"}");
        }

        /// <summary>
        /// Performs a POST request against api.warp.world/{username}/{endpoint} and automatically includes the user's token
        /// </summary>
        /// <param name="endpoint">the endpoint to hit</param>
        /// <param name="data">String representing the request body, should be JSON</param>
        /// <returns>The parsed response body, or a body with res.success=false</returns>
        public dynamic _post(string endpoint, string data)
        {

            var req = newRequestObject("POST", String.Format("{0}/{1}/{2}?token={3}", base_url, username, endpoint, token));

            var dataBytes = Encoding.UTF8.GetBytes(data);
            req.ContentType = "application/json";
            req.ContentLength = dataBytes.Length;

            using (var stream = req.GetRequestStream())
            {
                stream.Write(dataBytes, 0, data.Length);
            }

            try
            {
                var res = new StreamReader(req.GetResponse().GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.Linq.JObject.Parse(res);
            }
            catch (Exception ex)
            {
                log.Error("Error while connecting to warp world.", ex);
            }
            return Newtonsoft.Json.Linq.JObject.Parse("{\"success\":false, \"message\":\"Error connecting to warp world.\"}");
        }

        /// <summary>
        /// Wraps the update_entry endpoint which allows you to set the state of any entry in the queue.
        /// </summary>
        /// <param name="status">The status to set the entry to (won/loss/remove)</param>
        /// <param name="entry">Either the correct entry id, or 0 for the head of the queue.</param>
        /// <param name="queue">The active queue id</param>
        /// <returns>The response body</returns>
        public dynamic update_entry(string status, int entry, int queue)
        {
            string data = String.Format("{{\"status\":\"{0}\",\"entryID\":{1},\"queueID\":{2}}}", status, entry, queue);
            var res = _post("update_entry", data);
            log.Debug(String.Format("{0}", res));
            return res;
        }

        /// <summary>
        /// Obtains the currently active multiqueue for the user.
        /// </summary>
        /// <returns>The queue id or -1 on failure.</returns>
        public int getQueueId()
        {
            var info = _get("");
            return (int)info.multi_queue_information.queueID;
        }


        /// <summary>
        ///  Marks the head of the queue as a win
        /// </summary>
        public dynamic win()
        {
            var info = update_entry("won", 0, queue);
            if((bool)info.success)
            {
                log.Info("Marked level as won.");
            }
            return info;
        }

        /// <summary>
        /// Marks the head of the queue as a loss
        /// </summary>
        public dynamic lose()
        {
            var info = update_entry("loss", 0, queue);
            if ((bool)info.success)
            {
                log.Info("Marked level as a loss.");
            }
            return info;
        }
    }
}

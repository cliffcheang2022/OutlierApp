using System.Collections.Generic;

namespace OutlierApp.Model
{
    /// <summary>
    /// <para>Author      : Cliff Cheang</para>
    /// <para>Description : AppConfig field is corresponding to AppConfig.ini</para>
    /// </summary>
    public class AppConfig
    {
        public string _IPAddress { get; set; }
        public string _MacAddress { get; set; }
        public string _AppName { get; set; }
        public string _InputPath { get; set; }
        public string _OutputPath { get; set; }
        public string _BackupPath { get; set; }
        public string _InputFile { get; set; }
        public string _OutputFile { get; set; }
        public Server _DB_Server { get; set; }
        public Alert _AlertInstance { get; set; }

        public AppConfig()
        {
            _DB_Server = new Server();
            _AlertInstance = new Alert();
        }

        /// <summary>
        /// Server Class
        /// </summary>
        public class Server
        {
            public string Host { get; set; }
            public int Port { get; set; }
            /// <summary>
            /// Key for acknowledge server to validate connected clients
            /// </summary>
            public string ChannelKey { get; set; }

            public bool IsNull() => string.IsNullOrEmpty(Host);
        }


        public class Alert
        {
            public bool Enable { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
            public string From { get; set; }
            public List<string> To { get; set; }
            public List<string> Cc { get; set; }
            public string Subject { get; set; }
            public string Content { get; set; }
        }
    }
}

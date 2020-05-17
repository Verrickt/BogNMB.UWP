using BogNMB.API.Controllers;
using BogNMB.API.HTTPHandler;
using log4net;
using log4net.Config;
using log4net.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace BogNMB.API
{
    class ControllerConfigStore
    {
        private Dictionary<Type, string> _dict = new Dictionary<Type, string>();
        internal T GetConfigForController<T>(Type targetType)
        {
            string json = _dict[targetType];
            return JsonConvert.DeserializeObject<T>(json);
        }
        internal void SetConfigForController<T>(Type type, T value)
        {
            _dict[type] = JsonConvert.SerializeObject(value);
        }
    }

    public class ApiConfig
    {
        public string BaseUrl { get; set; } = "http://bog.ac/api/";
        public string DefaultUA { get; set; } = $"BogNMB.NetCore.API/{Assembly.GetExecutingAssembly().GetName().Version}";

        internal ControllerConfigStore ConfigStore { get; set; }

        public static bool operator ==(ApiConfig c1, ApiConfig c2) => c1.Equals(c2);
        public static bool operator !=(ApiConfig c1, ApiConfig c2) => !(c1 == c2);
        public override int GetHashCode()
        {
            return BaseUrl.GetHashCode();
        }


        static ApiConfig()
        {
            ILoggerRepository repository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
        }

        public HttpMessageHandler MessageHandler { get; set; } = new LoggingHttpHandler();

        public override bool Equals(object obj)
        {
            if (obj is ApiConfig config) return config.BaseUrl == this.BaseUrl;
            return false;
        }

        public static ApiConfig Default
        {
            get
            {
                var store = new ControllerConfigStore();
                var config = new ApiConfig() { BaseUrl = "http://bog.ac/api/",ConfigStore=store };
                store.SetConfigForController(typeof(PostController), 20);
                store.SetConfigForController(typeof(ThreadController), 20);
                return config;
            }
        }
        public static ApiConfig Test
        {
            get
            {
                var store = new ControllerConfigStore();
                var config = new ApiConfig() { BaseUrl = "http://test.bog.ac/api/", ConfigStore = store };
                store.SetConfigForController(typeof(PostController), 20);
                store.SetConfigForController(typeof(ThreadController), 5);
                return config;
            }
        }
    }
}

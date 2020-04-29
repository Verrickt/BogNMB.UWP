using BogNMB.API.HTTPHandler;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace BogNMB.API
{
	public class ApiConfig
	{
		public string BaseUrl { get; set; } = "http://bog.ac/api/";
		public string DefaultUA { get; set; } = $"BogNMB.NetCore.API/{Assembly.GetExecutingAssembly().GetName().Version}";

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

		public static ApiConfig Default => new ApiConfig() { BaseUrl = "http://bog.ac/api/" };
		public static ApiConfig Test => new ApiConfig() { BaseUrl = "http://test.bog.ac/api/" };
	}
}

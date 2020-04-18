using System;
using System.Reflection;

namespace BogNMB.API
{
	public class ApiConfig
	{
		public string BaseUrl { get; set; } = "http://bog.ac/api/";
		public string DefaultUA { get; set; } = $"BogNMB.NetCore.API/{Assembly.GetExecutingAssembly().GetName().Version}";
		private ApiConfig()
		{

		}
		public static ApiConfig Default => new ApiConfig() { BaseUrl = "http://bog.ac/api/" };
		public static ApiConfig Test = new ApiConfig() { BaseUrl = "http://test.bog.ac/api/" };
	}
}

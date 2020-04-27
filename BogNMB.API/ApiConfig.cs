using System;
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

		public override bool Equals(object obj)
		{
			if (obj is ApiConfig config) return config.BaseUrl == this.BaseUrl;
			return false;
		}

		public static ApiConfig Default => new ApiConfig() { BaseUrl = "http://bog.ac/api/" };
		public static ApiConfig Test => new ApiConfig() { BaseUrl = "http://test.bog.ac/api/" };
	}
}

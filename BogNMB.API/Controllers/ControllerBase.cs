using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BogNMB.API.Controllers
{
    public abstract class ControllerBase
    {
        private ApiConfig _config;
        private HttpClient _client;
        public abstract string ApiMode { get;  }

        private static Dictionary<ApiConfig, HttpClient> _map;

        static ControllerBase()
        {
            _map = new Dictionary<ApiConfig, HttpClient>();
        }

        public ControllerBase(ApiConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            if (!_map.ContainsKey(config)) { 
                var client = new HttpClient(config.MessageHandler);
                client.BaseAddress = new Uri(config.BaseUrl);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(config.DefaultUA);
                client.Timeout = TimeSpan.FromSeconds(10);
                _map[config] = client;
            }
            _client = _map[config];
            
        }
        private bool TryGetBogError(string str,out int errno)
        {
            var regex = "{\"error\":\"\\d*\"}";
            Regex r = new Regex(regex, RegexOptions.IgnoreCase);
            var match = r.Match(str, 0, Math.Min(50, str.Length));
            var anonymous = new { error = 1 };
            if (match.Success)
            {
                anonymous = JsonConvert.DeserializeAnonymousType(match.Value,anonymous);
            }
            errno = anonymous.error;
            return match.Success;
            
        }
        protected async Task<string> GetAsync(string p1,string p2)
        {
            HttpResponseMessage response = null;
            try
            {
                var path = string.Format("{0}\\{1}\\{2}", ApiMode, p1, p2);
                response = await _client.GetAsync(path).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var str = await response.Content.ReadAsStringAsync();
                if (TryGetBogError(str,out var errno))
                {
                    throw new BogException(errno);
                }
                return str;
                
            }
            catch (Exception ex) when (!(ex is BogException))
            {
                throw new BogException(response.ReasonPhrase, ex);
            }
            
        }
    }

    
}

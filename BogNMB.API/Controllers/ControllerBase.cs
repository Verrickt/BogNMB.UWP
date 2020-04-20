using Newtonsoft.Json;
using System;
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
        public ControllerBase(ApiConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _client = new HttpClient();
            _client.BaseAddress = new Uri(config.BaseUrl);
            _client.DefaultRequestHeaders.UserAgent.ParseAdd(config.DefaultUA);
            _client.Timeout = TimeSpan.FromSeconds(10);
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
            catch (Exception ex)
            {
                throw new BogException(response.ReasonPhrase, ex);
            }
            
        }
    }

    
}

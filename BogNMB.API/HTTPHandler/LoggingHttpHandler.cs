using log4net;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BogNMB.API.HTTPHandler
{
    public class LoggingHttpHandler : DelegatingHandler
    {
        private ILog Log = LogManager.GetLogger(typeof(LoggingHttpHandler));
        public LoggingHttpHandler(HttpMessageHandler handler):base(handler)
        {

        }
        public LoggingHttpHandler():base(new HttpClientHandler())
        {

        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var ts = DateTime.Now;
            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                var diff = DateTime.Now - ts;
                Log.Debug($"{request.Method} {request.RequestUri}->{diff.TotalMilliseconds}ms");
                return response;
            }
            catch (Exception ex)
            {
                Log.Error($"Error {request.Method} {request.RequestUri}", ex);
                throw;
            }
            
        }
    }
}

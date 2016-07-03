using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BSE365.Web;
using log4net;

namespace BSE365.Handlers
{
    public class LogRequestAndResponseHandler : DelegatingHandler
    {
        private ILog _logger = LogManager.GetLogger(typeof(Startup));

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // log auth
            var authInfo = request.Headers.Authorization;
            _logger.Info(authInfo);

            // log request url
            var requestUrl = request.RequestUri;
            _logger.Info(requestUrl);

            // log request body
            string requestBody = await request.Content.ReadAsStringAsync();
            _logger.Info(requestBody);

            // let other handlers process the request
            var result = await base.SendAsync(request, cancellationToken);

            if (result.Content != null)
            {
                // once response body is ready, log it
                var responseBody = await result.Content.ReadAsStringAsync();
                _logger.Info(responseBody);
            }

            return result;
        }
    }
}
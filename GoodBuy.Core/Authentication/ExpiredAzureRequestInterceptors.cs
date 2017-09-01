﻿using GoodBuy.Service;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GoodBuy
{
    class ExpiredAzureRequestInterceptors : DelegatingHandler
    {
        private readonly AzureService service;
        public ExpiredAzureRequestInterceptors(AzureService service) => this.service = service;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Clone the request, in case we need to re-issue it
            var clone = await CloneHttpRequestMessageAsync(request);
            // Now do the request
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // The request resulted in a 401 Unauthorized.  We need to do a LoginAsync,
                // which will do the Refresh if appropriate, or ask for credentials if not.
                await service.LoginAsync(Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider.Facebook);

                // Now, retry the request with the cloned request.  The only thing we have
                // to do is replace the X-ZUMO-AUTH header with the new auth token.
                clone.Headers.Remove("X-ZUMO-AUTH");
                clone.Headers.Add("X-ZUMO-AUTH", service.Client.CurrentUser.MobileServiceAuthenticationToken);
                response = await base.SendAsync(clone, cancellationToken);
            }

            return response;
        }

        public static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
        {
            HttpRequestMessage clone = new HttpRequestMessage(req.Method, req.RequestUri);

            // Copy the request's content (via a MemoryStream) into the cloned object
            var ms = new MemoryStream();
            if (req.Content != null)
            {
                await req.Content.CopyToAsync(ms).ConfigureAwait(false);
                ms.Position = 0;
                clone.Content = new StreamContent(ms);

                // Copy the content headers
                if (req.Content.Headers != null)
                    foreach (var h in req.Content.Headers)
                        clone.Content.Headers.Add(h.Key, h.Value);
            }

            clone.Version = req.Version;

            foreach (KeyValuePair<string, object> prop in req.Properties)
                clone.Properties.Add(prop);

            foreach (KeyValuePair<string, IEnumerable<string>> header in req.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }
    }
}

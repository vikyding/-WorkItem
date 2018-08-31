using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Msgraph.WorkItem
{
    class Program
    {
        static void Main()
        {
            MockRedirectHandler testHttpMessageHandler = new MockRedirectHandler();
            RetryHandler retryHandler = new RetryHandler(testHttpMessageHandler);
            HttpMessageInvoker invoker = new HttpMessageInvoker(retryHandler);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://example.org/foo");
            httpRequestMessage.Content = new StringContent("Hello World");

            var redirectResponse = new HttpResponseMessage((HttpStatusCode)429);
            redirectResponse.Headers.Add("Retry-After", 30.ToString());

            var response_2 = new HttpResponseMessage(HttpStatusCode.OK);

            testHttpMessageHandler.SetHttpResponse(redirectResponse, response_2);

            Task<HttpResponseMessage> response = invoker.SendAsync(httpRequestMessage, new CancellationToken());
        }
    }

    public class MockRedirectHandler : HttpMessageHandler
    {
        private HttpResponseMessage _response1
        { get; set; }
        private HttpResponseMessage _response2
        { get; set; }

        private bool _response1Sent = false;

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!_response1Sent)
            {
                _response1Sent = true;
                _response1.RequestMessage = request;
                return _response1;
            }
            else
            {
                _response1Sent = false;
                _response2.RequestMessage = request;
                return _response2;
            }
        }

        public void SetHttpResponse(HttpResponseMessage response1, HttpResponseMessage response2 = null)
        {
            this._response1Sent = false;
            this._response1 = response1;
            this._response2 = response2;
        }

    }

}

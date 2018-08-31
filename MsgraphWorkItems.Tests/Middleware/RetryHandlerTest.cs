using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MsgraphWorkItems.Tests
{
    using Mocks;
    using Msgraph.WorkItem;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Collections.Generic;

    [TestClass]
    public class RetryHandlerTest
    {
        private MockRedirectHander testHttpMessageHandler;
        private RetryHandler retryHandler;
        private HttpMessageInvoker invoker;
        private const string RETRY_AFTER = "Retry-After";
        private const string RETRY_ATTEMPT = "Retry-Attempt";

        [TestInitialize]
        public void Setup()
        {
            this.testHttpMessageHandler = new MockRedirectHander();
            this.retryHandler = new RetryHandler(this.testHttpMessageHandler);
            this.invoker = new HttpMessageInvoker(this.retryHandler);
        }

        [TestCleanup]
        public void Teardown()
        {
            this.invoker.Dispose();
        }

        [TestMethod]
        public void retryHandler_HttpMessageHandlerConstructor()
        {
            Assert.IsNotNull(retryHandler.InnerHandler, "HttpMessageHandler not initialized.");
            Assert.AreEqual(retryHandler.InnerHandler, testHttpMessageHandler, "Unexpected message handler set.");
            Assert.IsInstanceOfType(retryHandler, typeof(RetryHandler), "Unexpected redirect handler set.");
        }

        [TestMethod]
        public async Task OkStatusShouldPassThrough()
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://example.org/foo");

            var retryResponse = new HttpResponseMessage(HttpStatusCode.OK);
            this.testHttpMessageHandler.SetHttpResponse(retryResponse);

            var response = await this.invoker.SendAsync(httpRequestMessage, new CancellationToken());

            Assert.AreSame(response, retryResponse, "Return a successful response fail");
            Assert.AreSame(response.RequestMessage, httpRequestMessage, "Http response message sets request wrong.");
            Assert.IsFalse(response.RequestMessage.Headers.Contains(RETRY_ATTEMPT), "The request add header wrong.");

        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.ServiceUnavailable)]  // 503
        [DataRow(429)] // 429
        public async Task ShouldRetryWithAddRetryAttemptHeader(HttpStatusCode statusCode)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://example.org/foo");
            httpRequestMessage.Content = new StringContent("Hello World");

            var retryResponse = new HttpResponseMessage(statusCode);
            this.testHttpMessageHandler.SetHttpResponse(retryResponse);

            var response_2 = new HttpResponseMessage(HttpStatusCode.OK);

            this.testHttpMessageHandler.SetHttpResponse(retryResponse, response_2);

            var response = await invoker.SendAsync(httpRequestMessage, new CancellationToken());

            Assert.AreSame(response, response_2, "Return a response fail.");
            Assert.AreSame(response.RequestMessage, httpRequestMessage, "The request is set wrong.");
            Assert.IsNotNull(response.RequestMessage.Headers, "The request headers is null");
            Assert.IsTrue(response.RequestMessage.Headers.Contains(RETRY_ATTEMPT), "Doesn't set Retry-Attemp header to request");
            IEnumerable<string> values;
            Assert.IsTrue(response.RequestMessage.Headers.TryGetValues(RETRY_ATTEMPT, out values), "Get Retry-Attemp Header values");
            Assert.AreEqual(values.Count(), 1, "There are multiple values for Retry-Attemp header.");
            Assert.AreEqual(values.First(), 1.ToString(), "The value of  Retry-Attemp header is wrong.");
        }


        [DataTestMethod]
        [DataRow(HttpStatusCode.ServiceUnavailable)]  // 503
        [DataRow(429)] // 429
        public async Task RetryWithRetryAfterHeader(HttpStatusCode statusCode)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://example.org/foo");
            httpRequestMessage.Content = new StringContent("Hello World");

            var retryResponse = new HttpResponseMessage(statusCode);
            this.testHttpMessageHandler.SetHttpResponse(retryResponse);
            retryResponse.Headers.TryAddWithoutValidation(RETRY_AFTER, 20.ToString());

            var response_2 = new HttpResponseMessage(HttpStatusCode.OK);

            this.testHttpMessageHandler.SetHttpResponse(retryResponse, response_2);

            var response = await invoker.SendAsync(httpRequestMessage, new CancellationToken());

            Assert.IsNotNull(response.RequestMessage.Headers, "The request headers is null");
            Assert.IsTrue(response.RequestMessage.Headers.Contains(RETRY_ATTEMPT), "Doesn't set Retry-Attemp header to request");
            IEnumerable<string> values;
            Assert.IsTrue(response.RequestMessage.Headers.TryGetValues(RETRY_ATTEMPT, out values), "Get Retry-Attemp Header values");
            Assert.AreEqual(values.Count(), 1, "There are multiple values for Retry-Attemp header.");
            Assert.AreEqual(values.First(), 1.ToString(), "The value of  Retry-Attemp header is wrong.");
        }

    }



        
}

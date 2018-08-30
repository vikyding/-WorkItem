using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using Microsoft.WindowsAzure;


namespace Msgraph.WorkItem
{
    public class RetryHandler : DelegatingHandler
    {
        
        // property RetryPolicy
        //public RetryPolicy retryPolicy { get; private set; }

        //private const static int maxRetry;


        public RetryHandler()
        {
            throw new System.NotImplementedException();
        }

        public RetryHandler(HttpMessageHandler innerHandler)
        {
            InnerHandler = innerHandler;
        }


        public override Task<HttpResponseMessage> SendAsyncy(HttpRequestMessage HttpRequestMessage, CancellationToken CancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void SendAsyncyHttpRequestMessage(HttpRequestMessage, CancellationToken CancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}

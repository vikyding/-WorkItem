using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;


namespace Msgraph.WorkItem
{
    public class RetryHandler : DelegatingHandler
    {
        
        // property RetryPolicy
        //public RetryPolicy retryPolicy { get; private set; }

        private const int MAX_RETRY = 3;
        private const string RETRY_AFTER = "Retry-After";
        private const string RETRY_ATTEMPT = "Retry-Attempt";


        //public RetryHandler()
        //{
        //    throw new System.NotImplementedException();
        //}

        public RetryHandler(HttpMessageHandler innerHandler)
        {
            InnerHandler = innerHandler;
        }

        // public RetryHandler(HttpMessageHandler innerHandler, RetryPolicy retryPolicy){}

        /// <summary>
        /// Send a HTTP request 
        /// </summary>
        /// <param name="HttpRequestMessage">A HTTP request <see cref="HttpRequestMessage"/></param>
        /// <param name="CancellationToken"></param>
        /// <returns></returns>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken)
        {
            // Sends request first time
            var response = await base.SendAsync(httpRequest, cancellationToken);

            // Check response statusCode for whether needs retry
            if (IsRetry(response))
            {
                System.Diagnostics.Debug.WriteLine("do retry");
                response = await SendRetryAsync(response, cancellationToken);
            }

            return response;
        }

        public async Task<HttpResponseMessage> SendRetryAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {

            
            int retryCount = 0;

            TimeSpan delay = TimeSpan.FromSeconds(0);
            
            //Check if retry times less than maxRetry
            while (retryCount < MAX_RETRY)
            {
                
                var request = response.RequestMessage;
                retryCount++;
                AddOrUpdateRetryAttempt(request, retryCount);

                // Check response's header to get retry-after 
                HttpHeaders headers = response.Headers;
                if (headers.TryGetValues(RETRY_AFTER, out IEnumerable<string> values))
                {
                    string retry_after = values.First();
                    if (Int32.TryParse(retry_after, out int delay_seconds))
                    {
                        delay = TimeSpan.FromSeconds(delay_seconds);
                    }
                }
                else
                {
                    // Consider calculating an exponential delay here and
                    // using a strategy best suited for the operation and fault.
                }

                // Wait to retry the operation.
                await Task.Delay(delay);

                response = await base.SendAsync(request, cancellationToken);
                // Call base.SendAsyn to send the request

                if (!IsRetry(response)) {
                    System.Diagnostics.Debug.WriteLine("do retry again");
                    return response;
                }
                
            }
            // Throws ServiceException for TooManyTimes

            return response;
        }




        public bool IsRetry(HttpResponseMessage response)
        {
            if ((response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                response.StatusCode == (HttpStatusCode)429) && IsBuffed())
            {
                return true;
            }
            return false;
        }

        private bool IsBuffed() { return true; }

        private void AddOrUpdateRetryAttempt(HttpRequestMessage request, int retry_count)
        {
            if (request.Headers.Contains(RETRY_ATTEMPT)) {
                request.Headers.Remove(RETRY_ATTEMPT);
            }
            request.Headers.Add(RETRY_ATTEMPT, retry_count.ToString());
        }

    }
}

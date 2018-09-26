using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Microsoft.Graph;
using System.Reflection;
using System.Net.Http.Headers;

namespace Msgraph.WorkItem
{
    public static class GraphClientFactory
    {

        /// The key for the SDK version header.
        private static readonly string SdkVersionHeaderName = CoreConstants.Headers.SdkVersionHeaderName;
        
        /// The version for current assembly
        private static Version assemblyVersion = typeof(GraphClientFactory).GetTypeInfo().Assembly.GetName().Version;

        /// The value for the SDK version header.
        private static string SdkVersionHeaderValue = string.Format(
                    CoreConstants.Headers.SdkVersionHeaderValueFormatString,
                    "Graph",
                    assemblyVersion.Major,
                    assemblyVersion.Minor,
                    assemblyVersion.Build);


        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public static HttpClient CreateClient(DelegatingHandler[] handlers)
        {
            return Create(new HttpClientHandler(), handlers);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlers"></param>
        /// <param name="timeout"></param>
        /// <param name="baseAddress"></param>
        /// <returns></returns>
        public static HttpClient CreateClient(DelegatingHandler[] handlers, TimeSpan timeout, string baseAddress = null, CacheControlHeaderValue cacheControlHeaderValue = null)
        { 
            HttpClient client = Create(new HttpClientHandler(), handlers);
            if (timeout == null)
            {
                //throw error
            }
            return DefaultConfigure(client, timeout, baseAddress, cacheControlHeaderValue);
            
        }


      
        /// <summary>
        /// Creates a new <see cref="HttpClient"/> instance configured with the handlers provided and with the
        /// provided <paramref name="innerHandler"/> as the innermost handler.
        /// </summary>
        /// <param name="innerHandler">The inner handler represents the destination of the HTTP message channel.</param>
        /// <param name="handlers">An ordered list of <see cref="DelegatingHandler"/> instances to be invoked as an 
        /// <see cref="HttpRequestMessage"/> travels from the <see cref="HttpClient"/> to the network and an 
        /// <see cref="HttpResponseMessage"/> travels from the network back to <see cref="HttpClient"/>.
        /// The handlers are invoked in a top-down fashion. That is, the first entry is invoked first for 
        /// an outbound request message but last for an inbound response message.</param>
        /// <returns>An <see cref="HttpClient"/> instance with the configured handlers.</returns>
        public static HttpClient Create(HttpMessageHandler innerHanddler, params DelegatingHandler[] handlers)
        {
            HttpMessageHandler pipeline = CreatePipeline(innerHanddler, handlers);
            HttpClient client = new HttpClient(pipeline);
            client.DefaultRequestHeaders.Add(SdkVersionHeaderName, SdkVersionHeaderValue);
            return client;
        }

        /// <summary>
        /// Creates an instance of an <see cref="HttpMessageHandler"/> using the <see cref="DelegatingHandler"/> instances
        /// provided by <paramref name="handlers"/>. The resulting pipeline can be used to manually create <see cref="HttpClient"/>
        /// or <see cref="HttpMessageInvoker"/> instances with customized message handlers.
        /// </summary>
        /// <param name="innerHandler">The inner handler represents the destination of the HTTP message channel.</param>
        /// <param name="handlers">An ordered list of <see cref="DelegatingHandler"/> instances to be invoked as part 
        /// of sending an <see cref="HttpRequestMessage"/> and receiving an <see cref="HttpResponseMessage"/>.
        /// The handlers are invoked in a top-down fashion. That is, the first entry is invoked first for 
        /// an outbound request message but last for an inbound response message.</param>
        /// <returns>The HTTP message channel.</returns>
        public static HttpMessageHandler CreatePipeline(HttpMessageHandler innerHandler, IEnumerable<DelegatingHandler> handlers)
        {
            if (innerHandler == null)
            {
                // throw the error
            }

            if (handlers == null)
            {
                return innerHandler;
            }

            HttpMessageHandler pipeline = innerHandler;
            IEnumerable<DelegatingHandler> reversedHandlers = handlers.Reverse();
            foreach (DelegatingHandler handler in reversedHandlers)
            {
                if (handler == null)
                {
                    //throw the error
                }

                if (handler.InnerHandler != null)
                {
                    //throw the error
                }

                handler.InnerHandler = pipeline;
                pipeline = handler;
            }

            return pipeline;
        }

        private static HttpClient DefaultConfigure(HttpClient client, TimeSpan timeout, string baseAddress, CacheControlHeaderValue cacheControlHeaderValue)
        {
            try
            {
                client.Timeout = timeout;
            }
            catch (InvalidOperationException exception)
            {
                throw new ServiceException(
                    new Error
                    {
                        Code = ErrorConstants.Codes.NotAllowed,
                        Message = ErrorConstants.Messages.OverallTimeoutCannotBeSet,
                    },
                    exception);
            }

            if (baseAddress != null)
            {

            }

            client.DefaultRequestHeaders.CacheControl = cacheControlHeaderValue ?? new CacheControlHeaderValue { NoCache = true, NoStore = true };
            return client;
        }

        
    }








    // configure http proxy 
    // public ? SetHttpProxy(HttpHost proxy){}

    // configure sovereign could 
    // 
}

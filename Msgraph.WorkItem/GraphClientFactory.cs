using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msgraph.WorkItem
{
    class GraphClientFactory
    {

        // default
    }

    // GraphClientFactory implements IHttpClientFactory

    // provide pre-configured 

    // create Httt client with an array of delegation handlers as pipeline
    // public HttpClient CreateClient(DelegatingHandler[] handlers){}

    // create Http client with innermost handler
    // public HttpClient Create(HttpMessageHandler innerHanddler, DelegationHandler[] handlers){}

    // create pipeline
    // public static HttpMessageHandler CreatePipeline(){}

    // configure default Http handleing behavior
        // Set default request timeout
        // public TimeSpan OverallTimeout(){}

        // Override defalut base address
        // public SetBaseAddress(){}

        // Set default headers
        // public ? xxxHeader{set; get}

        // Set default serilizer
        // public ISerializer Serializer{get; set;}

   
    // configure http proxy 
    // public ? SetHttpProxy(HttpHost proxy){}

    // configure sovereign could 
    // 
}

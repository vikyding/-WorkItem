using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Msgraph.WorkItem
{
    public class Helper
    {
        static internal HttpRequestMessage CopyRequest(HttpRequestMessage originalRequest)
        {
            var newRequest = new HttpRequestMessage(originalRequest.Method, originalRequest.RequestUri);

            foreach (var header in originalRequest.Headers)
            {
                newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            foreach (var property in originalRequest.Properties)
            {
                newRequest.Properties.Add(property);
            }

            return newRequest;
        }
    }
}

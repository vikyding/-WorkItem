// ------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.
// ------------------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;
using Microsoft.Graph;

namespace Msgraph.WorkItem
{
    class Program
    {

        static void Main()
        {
            Console.WriteLine(CoreConstants.Headers.SdkVersionHeaderName);
            Console.WriteLine(GraphClientFactory.SdkVersionHeaderValue);
            Console.WriteLine(GraphClientFactory.sdkVersionHeaderName);

            string userInputAddress = Console.ReadLine();
        }
    }

  

}

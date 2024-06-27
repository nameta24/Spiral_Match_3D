using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Voodoo.Analytics;

namespace Voodoo.Sauce.Internal.Analytics.VoodooAnalytics._3rdParty.Common
{
    
    public class AnalyticsHttpLoggingClientHandler : DelegatingHandler
    {
        private const string TAG = "Analytics - HttpLogger - "; 
        public AnalyticsHttpLoggingClientHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestBody = "";
            if (request.Content != null)
            {
                requestBody = await request.Content.ReadAsStringAsync();
            }
            
            AnalyticsLog.Log(TAG, $"Request: {request} \nBody: {requestBody}");
            
            try {
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
                var responseBody = "";
                if (response.Content != null) {
                    responseBody = await response.Content.ReadAsStringAsync();
                }
                AnalyticsLog.Log(TAG, $"Response: {response} \nBody: {responseBody}");
                return response;
            } catch (Exception e) {
                AnalyticsLog.Log(TAG, $"API Call Error: "+e.Message);
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
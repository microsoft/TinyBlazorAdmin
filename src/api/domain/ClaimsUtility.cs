using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Cloud5mins.domain
{
    public static class ClaimsUtility
    {

        public static HttpStatusCode CatchUnauthorize(HttpRequestData req, ILogger log)
        {
            try{
                var tempReq = JsonSerializer.Serialize<HttpRequestData>(req);
                log.LogWarning($"===> ReqData: {tempReq}");

                ClaimsPrincipal principal = StaticWebAppsAuth.GetClaimsPrincipal(req,log);
                var temp = JsonSerializer.Serialize(principal);
                log.LogWarning($"===> principal: {temp}");
                

                if (principal == null)
                {
                    log.LogWarning("No principal.");
                    return HttpStatusCode.Unauthorized;
                }

                if(!principal.IsInRole("admin"))
                {
                    log.LogInformation("Not an admin");
                    return HttpStatusCode.Unauthorized;
                    
                }
                return HttpStatusCode.Continue;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "An unexpected error was encountered.");
                return HttpStatusCode.BadRequest;   
            }
        }
    }
}
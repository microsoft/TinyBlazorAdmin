using System;
using System.Collections.Generic;
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
                
                ClaimsPrincipal principal = StaticWebAppsAuth.GetClaimsPrincipal(req,log);              
  
                if (principal == null)
                {
                    log.LogTrace("No principal.");
                    return HttpStatusCode.Unauthorized;
                }

                if(!principal.IsInRole("admin"))
                {
                    log.LogInformation("Not IsInRole admin");
                    var claims = new List<Claim>( principal.FindAll(ClaimTypes.Role));
                    foreach(var c in claims){
                        if(c.Value == "admin")
                            return HttpStatusCode.Continue;
                    }
                    log.LogInformation("No claim with value admin");
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
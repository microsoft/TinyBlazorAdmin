/*
```c#
Input:
    {
         // [Required]
        "PartitionKey": "d",

         // [Required]
        "RowKey": "doc",

        // [Optional] New Title for this URL, or text description of your choice.
        "title": "Quickstart: Create your first function in Azure using Visual Studio"

        // [Optional] New long Url where the the user will be redirect
        "Url": "https://SOME_URL"
    }


Output:
    {
        "Url": "https://SOME_URL",
        "Clicks": 0,
        "PartitionKey": "d",
        "title": "Quickstart: Create your first function in Azure using Visual Studio"
        "RowKey": "doc",
        "Timestamp": "0001-01-01T00:00:00+00:00",
        "ETag": "W/\"datetime'2020-05-06T14%3A33%3A51.2639969Z'\""
    }
*/

using System;
using System.Threading.Tasks;
// using Microsoft.Azure.WebJobs;
// using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using Cloud5mins.domain;
using Cloud5mins.AzShortener;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace Cloud5mins.Function
{
    public class UrlUpdate
    {
        private readonly ILogger _logger;
        private readonly AdminApiSettings _adminApiSettings;

        public UrlUpdate(ILoggerFactory loggerFactory, AdminApiSettings settings)
        {
            _logger = loggerFactory.CreateLogger<UrlList>();
            _adminApiSettings = settings;
        }

        [Function("UrlUpdate")]
        public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req,
                                    ExecutionContext context
                                )
        {
            _logger.LogInformation($"HTTP trigger - UrlUpdate");

            string userId = string.Empty;
            ShortUrlEntity input;
            ShortUrlEntity result;

            try
            {
                var invalidCode = ClaimsUtility.CatchUnauthorize(req, _logger);
                if (invalidCode != HttpStatusCode.Continue)
                {
                    return req.CreateResponse(invalidCode);
                }

                // Validation of the inputs
                if (req == null)
                {
                    return req.CreateResponse(  HttpStatusCode.NotFound);
                }

                using (var reader = new StreamReader(req.Body))
                {
                    var strBody = reader.ReadToEnd();
                    input = JsonSerializer.Deserialize<ShortUrlEntity>(strBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (input == null)
                    {
                        return req.CreateResponse(  HttpStatusCode.NotFound);
                    }
                }

                // If the Url parameter only contains whitespaces or is empty return with BadRequest.
                if (string.IsNullOrWhiteSpace(input.Url))
                {
                    var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badRequest.WriteAsJsonAsync(new  { Message = "The url parameter can not be empty."} );    
                    return badRequest;     
                }

                // Validates if input.url is a valid aboslute url, aka is a complete refrence to the resource, ex: http(s)://google.com
                if (!Uri.IsWellFormedUriString(input.Url, UriKind.Absolute))
                {
                    var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badRequest.WriteAsJsonAsync(new  { Message = $"{input.Url} is not a valid absolute Url. The Url parameter must start with 'http://' or 'http://'."} );    
                    return badRequest;   
                }

                StorageTableHelper stgHelper = new StorageTableHelper(_adminApiSettings.UlsDataStorage);

                result = await stgHelper.UpdateShortUrlEntity(input);
                var host = string.IsNullOrEmpty(_adminApiSettings.customDomain) ? req.Url.Host : _adminApiSettings.customDomain.ToString();
                result.ShortUrl = Utility.GetShortUrl(host, result.RowKey);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error was encountered.");

                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteAsJsonAsync(new  { Message =  ex.Message} );    
                return badRequest;   
            }
       
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);   
            return response;    
        }
    }
}
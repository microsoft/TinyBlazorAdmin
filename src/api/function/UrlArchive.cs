/*
```c#
Input:
    {
         // [Required]
        "PartitionKey": "d",

         // [Required]
        "RowKey": "doc",

        // [Optional] all other properties
    }
Output:
    {
        "Url": "https://docs.microsoft.com/en-ca/azure/azure-functions/functions-create-your-first-function-visual-studio",
        "Title": "My Title",
        "ShortUrl": null,
        "Clicks": 0,
        "IsArchived": true,
        "PartitionKey": "a",
        "RowKey": "azFunc2",
        "Timestamp": "2020-07-23T06:22:33.852218-04:00",
        "ETag": "W/\"datetime'2020-07-23T10%3A24%3A51.3440526Z'\""
    }

*/

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Cloud5mins.domain;
using System.Threading;
using Cloud5mins.AzShortener;

namespace Cloud5mins.Function
{
    public class UrlArchive
    {

        private readonly ILogger _logger;
        private readonly AdminApiSettings _adminApiSettings;

        public UrlArchive(ILoggerFactory loggerFactory, AdminApiSettings settings)
        {
            _logger = loggerFactory.CreateLogger<UrlList>();
            _adminApiSettings = settings;
        }

        [Function("UrlArchive")]
        public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req,
        ExecutionContext context)
        {
            _logger.LogInformation($"HTTP trigger - UrlArchive");

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
                    var body = reader.ReadToEnd();
                    input = JsonSerializer.Deserialize<ShortUrlEntity>(body, new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
                    if (input == null)
                    {
                        return req.CreateResponse(  HttpStatusCode.NotFound);
                    }
                }

                StorageTableHelper stgHelper = new StorageTableHelper(_adminApiSettings.UlsDataStorage);

                result = await stgHelper.ArchiveShortUrlEntity(input);
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

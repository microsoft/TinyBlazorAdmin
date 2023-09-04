/*
```c#
Input:

    {
        // [Required] The url you wish to have a short version for
        "url": "https://docs.microsoft.com/en-ca/azure/azure-functions/functions-create-your-first-function-visual-studio",
        
        // [Optional] Title of the page, or text description of your choice.
        "title": "Quickstart: Create your first function in Azure using Visual Studio"

        // [Optional] the end of the URL. If nothing one will be generated for you.
        "vanity": "azFunc"
    }

Output:
    {
        "ShortUrl": "http://c5m.ca/azFunc",
        "LongUrl": "https://docs.microsoft.com/en-ca/azure/azure-functions/functions-create-your-first-function-visual-studio"
    }
*/

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.IO;
using System.Text.Json;
using System.Threading;

using Cloud5mins.domain;
using Cloud5mins.AzShortener;

namespace Cloud5mins.Function
{

    public class UrlShortener
    {
        private readonly ILogger _logger;
        private readonly AdminApiSettings _adminApiSettings;

        public UrlShortener(ILoggerFactory loggerFactory, AdminApiSettings settings)
        {
            _logger = loggerFactory.CreateLogger<UrlShortener>();
            _adminApiSettings = settings;
        }

        [Function("UrlShortener")]
        public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req,
            ExecutionContext context
        )
        {
            _logger.LogInformation($"__trace creating shortURL: {req}");
            string userId = string.Empty;
            ShortRequest input;
            var result = new ShortResponse();

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
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }

                using (var reader = new StreamReader(req.Body))
                {
                    var strBody = reader.ReadToEnd();
                    input = JsonSerializer.Deserialize<ShortRequest>(strBody, new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
                    if (input == null)
                    {
                        return req.CreateResponse(HttpStatusCode.NotFound);
                    }
                }

                // If the Url parameter only contains whitespaces or is empty return with BadRequest.
                if (string.IsNullOrWhiteSpace(input.Url))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new {Message = "The url parameter can not be empty."});
                    return badResponse;
                }

                // Validates if input.url is a valid aboslute url, aka is a complete refrence to the resource, ex: http(s)://google.com
                if (!Uri.IsWellFormedUriString(input.Url, UriKind.Absolute))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new {Message = $"{input.Url} is not a valid absolute Url. The Url parameter must start with 'http://' or 'http://'."});
                    return badResponse;
                }

                StorageTableHelper stgHelper = new StorageTableHelper(_adminApiSettings.UlsDataStorage);

                string longUrl = input.Url.Trim();
                string vanity = string.IsNullOrWhiteSpace(input.Vanity) ? "" : input.Vanity.Trim();
                string title = string.IsNullOrWhiteSpace(input.Title) ? "" : input.Title.Trim();


                ShortUrlEntity newRow;

                if (!string.IsNullOrEmpty(vanity))
                {
                    newRow = new ShortUrlEntity(longUrl, vanity, title, input.Schedules);
                    if (await stgHelper.IfShortUrlEntityExist(newRow))
                    {
                        var badResponse = req.CreateResponse(HttpStatusCode.Conflict);
                        await badResponse.WriteAsJsonAsync(new {Message = "This Short URL already exist."});
                        return badResponse;
                    }
                }
                else
                {
                    newRow = new ShortUrlEntity(longUrl, await Utility.GetValidEndUrl(vanity, stgHelper), title, input.Schedules);
                }

                await stgHelper.SaveShortUrlEntity(newRow);

                var host = string.IsNullOrEmpty(_adminApiSettings.customDomain) ? req.Url.Host: _adminApiSettings.customDomain.ToString();
                result = new ShortResponse(host, newRow.Url, newRow.RowKey, newRow.Title);

                _logger.LogInformation("Short Url created.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error was encountered.");

                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new {Message = ex.Message});
                return badResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);

            return response;
        }
    }
}

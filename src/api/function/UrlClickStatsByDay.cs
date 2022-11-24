/*
```c#
Input:

    {
        // [Required] the end of the URL that you want statistics for.
        "vanity": "azFunc"
    }

Output:
    {
    "items": [
        {
        "dateClicked": "2020-12-19",
        "count": 1
        },
        {
        "dateClicked": "2020-12-03",
        "count": 2
        }
    ],
    "url": ""https://c5m.ca/29"
*/

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net;
using Cloud5mins.domain;
using System.Security.Claims;
using System.Text.Json;
using System.IO;
using System.Linq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading;
using Cloud5mins.AzShortener;

namespace Cloud5mins.Function
{
    public class UrlClickStatsByDay
    {
        private readonly ILogger _logger;
        private readonly AdminApiSettings _adminApiSettings;

        public UrlClickStatsByDay(ILoggerFactory loggerFactory, AdminApiSettings settings)
        {
            _logger = loggerFactory.CreateLogger<UrlClickStatsByDay>();
            _adminApiSettings = settings;
        }

        [Function("UrlClickStatsByDay")]
        public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req,
        ExecutionContext context)
        {
            _logger.LogInformation($"HTTP trigger: UrlClickStatsByDay");

            string userId = string.Empty;
            UrlClickStatsRequest input;
            var result = new ClickDateList();

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

            try
            {
                using (var reader = new StreamReader(req.Body))
                {
                    var strBody = reader.ReadToEnd();
                    input = JsonSerializer.Deserialize<UrlClickStatsRequest>(strBody, new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
                    if (input == null)
                    {
                        return req.CreateResponse(  HttpStatusCode.NotFound);
                    }
                }

                StorageTableHelper stgHelper = new StorageTableHelper(_adminApiSettings.UlsDataStorage);

                var rawStats = await stgHelper.GetAllStatsByVanity(input.Vanity);
                
                result.Items = rawStats.GroupBy( s => DateTime.Parse(s.Datetime).Date)
                                            .Select(stat => new ClickDate{
                                                DateClicked = stat.Key.ToString("yyyy-MM-dd"),
                                                Count = stat.Count()
                                            }).OrderBy(s => DateTime.Parse(s.DateClicked).Date).ToList<ClickDate>(); 

                var host = string.IsNullOrEmpty(_adminApiSettings.customDomain) ? req.Url.Host: _adminApiSettings.customDomain.ToString();
                result.Url = Utility.GetShortUrl(host, input.Vanity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error was encountered.");
                 var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                 await badRequest.WriteAsJsonAsync(new  { Message = $"{ex.Message}"} );    
                return badRequest;   
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);   
            return response;    
        }
    }
}

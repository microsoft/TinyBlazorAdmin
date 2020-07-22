
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components;
using System.Text;
using System.Net.Http.Json;

namespace TinyBlazorAdmin.Data
{
    /// <summary>
    /// Client to fetch the Cosmos DB token.
    /// </summary>
    public class UrlShortenerSecuredService
    {
        /// <summary>
        /// An <see cref="HttpClient"/> instance configured to securely access
        /// the functions endpoint.
        /// </summary>
        private readonly HttpClient _client;



        private static StringContent CreateHttpContent(object content)
        {
            StringContent httpContent = null;

            if (content != null)
            {
                var jsonString = JsonConvert.SerializeObject(content);
                httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            }

            return httpContent;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="UrlShortenerSecuredService"/> class.
        /// </summary>
        /// <param name="factory"></param>
        public UrlShortenerSecuredService(IHttpClientFactory factory)
        {
            _client = factory.CreateClient(nameof(UrlShortenerSecuredService));
        }


        public async Task<ShortUrlList> GetUrlList()
        {
            string result = string.Empty;
            var resultList = await _client.GetJsonAsync<ShortUrlList>($"/api/UrlList?code=qzmB9P2p1ujT/98a16WlR1XvMlsBBJlKKcv5lZsH0cpR4f/RGqAnXQ==");
            return resultList;
        }

        public async Task<ShortUrlList> CreateShortUrl(ShortUrlRequest shortUrlRequest)
        {
            CancellationToken cancellationToken;

            var response = await _client.PostAsJsonAsync($"/api/UrlShortener?code=qzmB9P2p1ujT/98a16WlR1XvMlsBBJlKKcv5lZsH0cpR4f/RGqAnXQ==", shortUrlRequest, cancellationToken);
            
            var resultList = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ShortUrlList>(resultList);
        }


        public async Task<ShortUrlEntity> UpdateShortUrl(ShortUrlEntity editedUrl)
        {
            CancellationToken cancellationToken;

            var response = await _client.PostAsJsonAsync($"/api/UrlUpdate?code=DClqzYbsW3cZFbbLIkINEdZI2kp6js3fqILw6Ei8CwLlmgISSih6Ug==", editedUrl, cancellationToken);
            
            var resultList = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ShortUrlEntity>(resultList);
        }




        public async Task<ShortUrlEntity> ArchiveShortUrl(ShortUrlEntity archivedUrl) 
        {
            CancellationToken cancellationToken;

            var response = await _client.PostAsJsonAsync($"/api/UrlArchive?code=8m22e/aFQv5Qa3ovxBc7f2m9eqRVPnLp1bpPJhHv1Z5HNzgUJ/fhPg==", archivedUrl, cancellationToken);
            
            var resultList = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ShortUrlEntity>(resultList);

        }
    }




}
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TinyBlazorAdmin.Data;

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

            // try{
                //var obj = "{'name':'"+ name + "'}";

                //var content = new StringContent(obj, Encoding.UTF8, "application/json");
                //var response = await _client.SendAsync($"api/UrlList?code=6su5tN2rzQn0dW6yq/YVDKpCnD3zN0khgshEOm7qhwfSg1MRNNxG3Q=="); 
                
                //result = await response.Content.ReadAsStringAsync();
                //result = JsonConvert.DeserializeObject<string>(resultList);

                CancellationToken cancellationToken;

                using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/UrlList?code=6su5tN2rzQn0dW6yq/YVDKpCnD3zN0khgshEOm7qhwfSg1MRNNxG3Q=="))
                {
                    using (var response = await _client
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                        .ConfigureAwait(false))
                    {
                        var resultList = response.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject<ShortUrlList>(resultList);
                    }
                }
            // }
            // catch(Exception ex ){
            //     result = ex.Message;
            // }
        }
    }
}



        //     CancellationToken cancellationToken;

        //     using (var client = new HttpClient())
        //     using (var request = new HttpRequestMessage(HttpMethod.Get, url))
        //     {
        //         using (var response = await client
        //             .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
        //             .ConfigureAwait(false))
        //         {
        //             var resultList = response.Content.ReadAsStringAsync().Result;
        //             return JsonConvert.DeserializeObject<ShortUrlList>(resultList);
        //         }
        //     }
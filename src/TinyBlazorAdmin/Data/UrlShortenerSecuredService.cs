using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


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

        /// <summary>
        /// Gets a new <see cref="AzToken"/> based on the authenticated user.
        /// </summary>
        /// <returns>A new <see cref="AzToken"/> instance.</returns>
        public async Task<string> GetWhatsNew(string name)
        {
            string result = string.Empty;

            try{
                var obj = "{'name':'"+ name + "'}";

                var content = new StringContent(obj, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"api/WhatsNew", content); 
                
                result = await response.Content.ReadAsStringAsync();
                //result = JsonConvert.DeserializeObject<string>(resultList);
            }
            catch(Exception ex ){
                result = ex.Message;
            }

            return result;
        }
    }
}
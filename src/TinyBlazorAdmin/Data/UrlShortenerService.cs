using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;

namespace tinyBlazorAdmin.Data
{
    public class UrlShortenerService
    {
        public IConfiguration Config { get; set; }
        public UrlShortenerService(IConfiguration config)
        {
            //Config = GetConfiguration();
            Config = config;
        }

        //private static IConfigurationRoot GetConfiguration()
        //{
        //    var config = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
        //       // .AddEnvironmentVariables()
        //        .Build();
        //    return config;
        //}

        private async Task<string> GetFunctionUrl(string functionName){


            StringBuilder FuncUrl = new StringBuilder(await GetSecret("AzFunctionURL"));
            FuncUrl.Append("/api/");
            FuncUrl.Append(functionName);

            string code = await GetSecret("AzFunctionCode");
            if(!string.IsNullOrWhiteSpace(code))
            {
                FuncUrl.Append("?code=");
                FuncUrl.Append(code);
            }
            
            return FuncUrl.ToString();
        }

        private async Task<string> GetSecret(string secretName){

            var tokenService = new AzureServiceTokenProvider();
            var keyVaultURL = Config.GetSection("KeyVault")["keyVaultURL"];
            var kvault = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(tokenService.KeyVaultTokenCallback));
            //string secret = (await kvault.GetSecretAsync(GetConfiguration()["keyVaultURL"], secretName)).Value;
            string secret = (await kvault.GetSecretAsync(keyVaultURL, secretName)).Value;

            return secret;
        }

        public async Task<ShortUrlList> GetUrlList()
        {
            var url = await GetFunctionUrl("UrlList");

            CancellationToken cancellationToken;

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (var response = await client
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false))
                {
                    var resultList = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<ShortUrlList>(resultList);
                }
            }
        }


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

        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }
    }
}

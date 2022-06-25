using System.Net;
using System.Reflection;
using System.Text.Json;
using BSXIV.Utilities;
using Newtonsoft.Json;

namespace BSXIV.Utilities
{
    public class WebRequest
    {
        private HttpClient _httpClient;
        private LoggingUtils _logging;

        public WebRequest(LoggingUtils logging)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", 
                $"{Constants.AppName}/{GetType().Assembly.GetName().Version}");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _logging = logging;
        }

        internal async Task<string> MakeRequestAsync(string url)
        {
            return await MakeRequestAsync(url, CancellationToken.None);
        }

        internal async Task<string> MakeRequestAsync(string url, CancellationToken cancellationToken, 
            Action<float> progressCallback = null!)
        {
            var webRequest = _httpClient.GetAsync(url, cancellationToken);
            string responseStream = null!;
            try
            {
                var response = await webRequest.ConfigureAwait(false);
                responseStream = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                
            }
            catch (Exception e)
            {
                await _logging.Log(LogSeverity.Error, e.Message);
            }

            return responseStream;
        }
    }
}
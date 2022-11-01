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

        internal async Task<Stream?> MakeRequestAsync(string url) => await MakeRequestAsync(new Uri(url));

        internal async Task<Stream?> MakeRequestAsync(Uri url)
        {
            return await MakeRequestAsync(url, CancellationToken.None);
        }

        internal async Task<Stream?> MakeRequestAsync(string url, CancellationToken cancellationToken, Action<float> progressCallback = null!) => await MakeRequestAsync(new Uri(url), cancellationToken, progressCallback);

        internal async Task<Stream?> MakeRequestAsync(Uri url, CancellationToken cancellationToken, 
            Action<float> progressCallback = null!)
        {
            // await _logging.Log(LogSeverity.Debug, url);
            var webRequest = _httpClient.GetAsync(url, cancellationToken);
            Stream? responseStream = null;
            try
            {
                var response = await webRequest.ConfigureAwait(false);
                await _logging.Log(LogSeverity.Debug, response.StatusCode.ToString());
                if(response.IsSuccessStatusCode)
                    responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                
            }
            catch (Exception e)
            {
                await _logging.Log(LogSeverity.Error, e.Message);
            }

            return responseStream;
        }
    }
}
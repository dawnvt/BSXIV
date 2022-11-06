using System.Net;
using System.Reflection;
using System.Text.Json;
using BSXIV.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BSXIV.Utilities
{
    public class WebRequest
    {
        private HttpClient _httpClient;
        private ILogger _logger;

        public WebRequest(ILogger<WebRequest> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", 
                $"{Constants.AppName}/{GetType().Assembly.GetName().Version}");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            
        }

        internal async Task<Stream?> MakeRequestAsync(string url) => await MakeRequestAsync(new Uri(url));

        internal async Task<Stream?> MakeRequestAsync(Uri url)
        {
            return await MakeRequestAsync(url, CancellationToken.None);
        }

        internal async Task<Stream?> MakeRequestAsync(string url, CancellationToken cancellationToken, Action<float> progressCallback = null!) => await MakeRequestAsync(new Uri(url), cancellationToken, progressCallback);

        internal async Task<Stream?> MakeRequestAsync(Uri url, CancellationToken cancellationToken, Action<float> progressCallback = null!)
        {
            Stream? responseStream = null;
            try
            {
                var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                    responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error making request to {url}", url);
            }

            return responseStream;
        }
    }
}
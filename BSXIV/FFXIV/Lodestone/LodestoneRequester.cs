using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BSXIV.FFXIV.Lodestone
{
    public class LodestoneRequester
    {
        private HttpClient _desktop;
        private HttpClient _mobile;

        public LodestoneRequester()
        {
            _desktop = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    UserAgent =
                    {
                        new ProductInfoHeaderValue("curl", "7.73.0")
                    }
                }
            };
            _mobile = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    UserAgent =
                    {
                        new ProductInfoHeaderValue("Mozilla", "5.0 (iPhone; CPU OS 10_15_5 like Mac OS X)"),
                        new ProductInfoHeaderValue("AppleWebKit", "605.1.15 (KHTML, like Gecko)"),
                        new ProductInfoHeaderValue("Version", "12.1.1"),
                        new ProductInfoHeaderValue("Mobile", "14E304"),
                        new ProductInfoHeaderValue("Safari", "605.1.15")
                    }
                }
            };
        }

        public async Task<string> GetDesktop(string url)
        {
            return await _desktop.GetStringAsync(url);
        }

        public async Task<string> GetMobile(string url)
        {
            return await _mobile.GetStringAsync(url);
        }
    }

    public class LodestoneSelectors
    {
        [JsonProperty("selector")]
        public string Selector { get; set; }
        [JsonProperty("attribute")]
        public string? Attribute { get; set; }
        [JsonProperty("regex")]
        public string? Regex { get; set; }
        [JsonProperty("multiple")]
        public bool? Multiple { get; set; }
    }
}

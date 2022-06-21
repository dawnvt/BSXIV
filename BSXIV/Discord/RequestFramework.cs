namespace BSXIV.Discord
{
    public abstract class RequestFramework
    {
        // Abstractions
        public abstract string USER_AGENT { get; set; }
        
        // Private Variables
        private HttpClient _HttpClient;
        
        public RequestFramework()
        {
            _HttpClient = new HttpClient();
            _HttpClient.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
        }

        public async Task<string> GetUserInfo(string url, CancellationToken cancellationToken)
        {
            var response = await _HttpClient.GetAsync(url, cancellationToken);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
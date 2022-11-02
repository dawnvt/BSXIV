using Microsoft.Extensions.Logging;

namespace BSXIV.Utilities
{
    public class Log
    {
        private readonly ILogger<Log> _logger;

        public Log(ILogger<Log> logger)
        {
            _logger = logger;
        }
    }
}
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

        public void Action(string name)
        {
            _logger.Log((LogLevel)20,"Just doing stuff!", name);
        }
    }
}
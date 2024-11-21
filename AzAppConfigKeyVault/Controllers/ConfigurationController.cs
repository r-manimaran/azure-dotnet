using AzAppConfigKeyVault;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IOptionsMonitor<AppConfOptions> _options;

        public ConfigurationController(IOptionsMonitor<AppConfOptions> options)
        {
            _options = options;
        }

        [HttpGet]
        public IEnumerable<string> GetConfiguration()
        {

            return new string[] 
            { 
                _options.CurrentValue.FirstConfig,
                _options.CurrentValue.SecondConfig
            };
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VS_AzureAppService_Trainer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaultController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public VaultController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Environment = Environment.OSVersion.ToString(),
                ConfidentialAppTestSecret = _configuration["ConfidentialAppTestSetting"]
            });
        }
    }
}

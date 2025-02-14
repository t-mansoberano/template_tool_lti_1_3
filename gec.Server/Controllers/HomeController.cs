using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using gec.Models.Common;
using gec.ServerIdentidad;
using Microsoft.Extensions.Logging;

namespace InterfazUnica.Server.Controllers
{
    [ExcludeFromCodeCoverage]
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly AuthHelper AuthHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HomeController>  _logger;

        public HomeController(IConfiguration configuration, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor, ILogger<HomeController> logger)
        {
            AuthHelper = new AuthHelper(configuration, environment, httpContextAccessor);
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpGet("GetUserClaims")]
        public UserClaims GetUserClaims()
        {
            _logger.LogInformation("1.-GetUserClaims");
            return AuthHelper.GetClaims();
        }
    }
}


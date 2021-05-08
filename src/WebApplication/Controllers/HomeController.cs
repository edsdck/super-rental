using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication.Extensions;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGatewayService _gatewayService;
        private readonly IConfiguration _configuration;

        public HomeController(IGatewayService gatewayService,
            IConfiguration configuration)
        {
            _gatewayService = gatewayService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Login() => RedirectToAction(nameof(Index));

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var configuration = _configuration.GetSection("IdentityServer");
            
            return View(new RegisterViewModel
            {
                ReturnUrl = configuration["RedirectAfterRegister"]
            });
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _gatewayService.Post("account/register", model);

            return result.IsSuccess ?
                Redirect(model.ReturnUrl) :
                View(model).WithWarning("Oof", "Registracija nepavyko");
        }

        public IActionResult Secure() => View();
        
        public IActionResult Logout() => SignOut("cookie", "oidc");

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
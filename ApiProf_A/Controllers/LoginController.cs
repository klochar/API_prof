using Microsoft.AspNetCore.Mvc;

namespace ApiProf_A.Controllers
{
    
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

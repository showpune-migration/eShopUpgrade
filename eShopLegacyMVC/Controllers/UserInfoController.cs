using Microsoft.AspNetCore.Mvc;


namespace eShopLegacyMVC.Controllers
{
    public class UserInfoController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}
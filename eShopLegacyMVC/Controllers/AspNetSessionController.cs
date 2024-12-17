using eShopLegacy.Models;
using Microsoft.AspNetCore.Mvc;


namespace eShopLegacyMVC.Controllers
{
    public class AspNetSessionController : Controller
    {
        // GET: AspNetCoreSession
        public ActionResult Index()
        {
            var model = HttpContext.Session["DemoItem"];
            return View(model);
        }

        // POST: AspNetCoreSession
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SessionDemoModel demoModel)
        {
            HttpContext.Session["DemoItem"] = demoModel;
            return View(demoModel);
        }
    }
}

using eShopLegacy.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace eShopLegacyMVC.Controllers
{
    public class AspNetSessionController : Controller
    {
        // GET: AspNetCoreSession
        public IActionResult Index()
        {
            var modelJson = HttpContext.Session.GetString("DemoItem");
            var model = modelJson != null ? JsonSerializer.Deserialize<SessionDemoModel>(modelJson) : null;
            return View(model);
        }

        // POST: AspNetCoreSession
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SessionDemoModel demoModel)
        {
            HttpContext.Session.SetString("DemoItem", JsonSerializer.Serialize(demoModel));
            return View(demoModel);
        }
    }
}
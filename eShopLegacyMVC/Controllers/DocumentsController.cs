using eShopLegacyMVC.Services;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace eShopLegacyMVC.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        public DocumentsController()
        {
            _contentTypeProvider = new FileExtensionContentTypeProvider();
        }

        // GET: Files
        public ActionResult Index()
        {
            var files = FileService.Create().ListFiles();
            return View(files);
        }

        [ResponseCache(VaryByQueryKeys = new[] { "filename" }, Duration = int.MaxValue)]
        public FileResult Download(string filename)
        {
            var fileService = FileService.Create();
            var file = fileService.DownloadFile(filename);

            if (!_contentTypeProvider.TryGetContentType(filename, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            FileContentResult fc = new FileContentResult(file, contentType);
            fc.FileDownloadName = filename;
            return fc;
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadDocument()
        {
            var fileService = FileService.Create();
            fileService.UploadFile(Request.Files);
            return RedirectToAction("Index");
        }
    }
}
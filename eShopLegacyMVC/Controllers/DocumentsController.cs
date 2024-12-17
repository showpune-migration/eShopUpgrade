using eShopLegacyMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace eShopLegacyMVC.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IFileService _fileService;

        public DocumentsController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // GET: Files
        public ActionResult Index()
        {
            var files = _fileService.ListFiles();
            return View(files);
        }

        [ResponseCache(VaryByQueryKeys = new[] { "filename" }, Duration = int.MaxValue)]
        public FileResult Download(string filename)
        {
            var file = _fileService.DownloadFile(filename);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filename, out string contentType))
            {
                contentType = "application/octet-stream";
            }
            return File(file, contentType, filename);
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadDocument(IFormFileCollection files)
        {
            _fileService.UploadFile(files);
            return RedirectToAction("Index");
        }
    }
}
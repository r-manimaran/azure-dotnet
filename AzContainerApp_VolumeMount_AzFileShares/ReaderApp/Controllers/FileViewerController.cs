using Microsoft.AspNetCore.Mvc;

namespace ReaderApp.Controllers
{
    public class FileViewerController : Controller
    {
        public IActionResult Index()
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "images");
            ViewBag.Files = Directory.EnumerateFiles(directory)
                                     .Select(x=>new FileInfo(x).Name)
                                     .ToList();
            
            return View();
        }
    }
}

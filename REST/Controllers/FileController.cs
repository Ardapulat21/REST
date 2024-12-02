using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST.Models;
using FileInfo = REST.Models.FileInfo;
namespace REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly string? _resourcesFolder;
        public FileController(IConfiguration configuration)
        {
            _resourcesFolder = configuration["ResourceFolder"];
            if (!String.IsNullOrEmpty(_resourcesFolder) && !Directory.Exists(_resourcesFolder))
            {
                Directory.CreateDirectory(_resourcesFolder);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded." });
            }
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                return BadRequest(new { error = "Only .jpg and .png files are allowed." });
            }
            var filePath = Path.Combine(_resourcesFolder, Path.GetFileName(file.FileName));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Json(new { fileName = file.FileName });
        }
        [HttpGet]
        [Route("Fetch")]
        public ActionResult<List<FileInfo>> Fetch(){
            var files = new List<FileInfo>();
            string filename;
            if (Directory.Exists(_resourcesFolder))
            {
                var fileEntries = Directory.GetFiles(_resourcesFolder);
                foreach (var filePath in fileEntries)
                {
                    filename = Path.GetFileName(filePath);
                    if(Path.GetExtension(filePath) != ".jpg" && Path.GetExtension(filePath) != ".png")
                        continue;   
                        
                    files.Add(new FileInfo
                    {
                        Filename = filename,
                        FilePath = Path.Combine("..","..","Resources",filename) // URL for accessing the file
                    });
                }
            }
            return Ok(files);
        }
    }
}
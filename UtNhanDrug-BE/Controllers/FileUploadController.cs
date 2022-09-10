using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using UtNhanDrug_BE.Helper;

namespace UtNhanDrug_BE.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class FileUploadController : ControllerBase
    {
        private readonly UploadFile _uploadFile;
        public FileUploadController(UploadFile uploadFile)
        {
            _uploadFile = uploadFile;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public ActionResult UploadFile(IFormFile file)
        {
            FileStream stream;
            string fileName = file.FileName;
            FileInfo f = new FileInfo(file.FileName);
            string path = f.FullName;

            var filePath = Path.Combine(path);

            stream = new FileStream(filePath, FileMode.Create);

            _uploadFile.Upload(stream, fileName);
            return Ok();
        }
    }
}

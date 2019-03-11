using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Http.Headers;

namespace DatNenWebApi.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private static readonly string UploadFolderName = "Upload";

        public FileController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost, DisableRequestSizeLimit]
        public string UploadFile([FromBody]string content)
        {
            try
            {
                string folderName = UploadFolderName;
                string webRootPath = _hostingEnvironment.ContentRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                var bytes = Convert.FromBase64String(content);
                
                {
                    string fullPath = Path.Combine(newPath, "ahihi.pdf");
                    System.IO.File.WriteAllBytes(fullPath, bytes);
                }
                return "Upload Successful.";
            }
            catch (System.Exception ex)
            {
                return "Upload Failed: " + ex.Message;
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        public string UploadRaw([FromForm]IFormFileCollection files)
        {
            try
            {
                var file = files[0];
                string folderName = UploadFolderName;
                string webRootPath = _hostingEnvironment.ContentRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                return "Upload Successful.";
            }
            catch (System.Exception ex)
            {
                return "Upload Failed: " + ex.Message;
            }
        }

        [HttpGet]
        [Route("api/[controller]")]
        public FileResult downloadFile(string fileName)
        {
            string folderName = UploadFolderName;
            string webRootPath = _hostingEnvironment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            var fullPath = Path.Combine(newPath, fileName);
            var stream = new FileStream(fullPath, FileMode.Open);
            var mimeType = "application/x-msdownload";
            return File(stream, mimeType, fileName);
        }

        [HttpDelete("{filename}")]
        public void Delete(string filename)
        {
            string folderName = UploadFolderName;
            string webRootPath = _hostingEnvironment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            System.IO.File.Delete(Path.Combine(newPath, filename));
        }
    }
}
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TekDesk.Controllers
{
    public class DownloadController : Controller
    {
        private IWebHostEnvironment _env;

        public DownloadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        //GET api/download/12345abc
        public IActionResult Download(string filename)
        {
            var filepath = _env.ContentRootPath + "\\FileUploads\\" + filename;

            var net = new System.Net.WebClient();
            var data = net.DownloadData(filepath);

            var stream = new System.IO.MemoryStream(data);

            if (stream == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(stream, "application/octet-stream", filename); // returns a FileStreamResult
   

        }
    }
}

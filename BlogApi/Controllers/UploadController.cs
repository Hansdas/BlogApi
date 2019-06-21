using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
namespace BlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private IHostingEnvironment hostingEnvironment;
        public UploadController(IHostingEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
        }
        [HttpPost]
        public IActionResult Upload()
        {
            try
            {
                var imgFile = Request.Form.Files[0];
                int index = imgFile.FileName.LastIndexOf('.');
                //获取后缀名
                string extension = imgFile.FileName.Substring(index, imgFile.FileName.Length - index);
                string webpath = hostingEnvironment.ContentRootPath;
                string guid = Guid.NewGuid().ToString().Replace("-", "");
                string newFileName = guid + extension;
                DateTime dateTime = DateTime.Now;
                //linux环境目录为/{1}/
                string path = string.Format(@"{0}/TemporaryFile/{1}/{2}/{3}", "/home/www", dateTime.Year.ToString(), dateTime.Month.ToString()
                    , dateTime.Day.ToString());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string imgSrc = path + @"/" + newFileName;
                using (FileStream fs = System.IO.File.Create(imgSrc))
                {
                    imgFile.CopyTo(fs);
                    fs.Flush();
                }
                return new JsonResult(new { message = "OK", code = 200 });
            }
            catch (Exception e)
            {
                return new JsonResult(new {message=e.Message,code=500});
            }
        }
        [HttpDelete]
        public string DeleteFile()
        {
            return "1";
        }
    }
}
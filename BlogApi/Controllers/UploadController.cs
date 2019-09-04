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
        public UploadController()
        {
        }
        [HttpPost]
        public IActionResult Upload()
        {
            var imgFile = Request.Form.Files[0];
            string fileName = imgFile.FileName;
            DateTime dateTime = DateTime.Now;
            //linux环境目录为/{1}/
            string path = string.Format(@"{0}/TempFile/{1}/{2}/{3}", "/home/www", dateTime.Year.ToString(), dateTime.Month.ToString()
                , dateTime.Day.ToString());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string savePath = path + @"/" + fileName;
            try
            {              
                using (FileStream fs = System.IO.File.Create(savePath))
                {
                    imgFile.CopyTo(fs);
                    fs.Flush();
                }
                return new JsonResult(new { savepath = savePath, code = 200 });
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
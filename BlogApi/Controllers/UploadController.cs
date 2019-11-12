using System;
using Microsoft.AspNetCore.Mvc;
using System.IO;
namespace BlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [HttpGet("{path}")]
        public string TestGet(string path)
        {
            return path;
        }
        [HttpPost]
        public IActionResult Upload()
        {
            var imgFile = Request.Form.Files[0];
            string fileName = imgFile.FileName;
            DateTime dateTime = DateTime.Now;
            //路径日期部分
            string datePath = string.Format("{0}/{1}/{2}/", dateTime.Year.ToString(), dateTime.Month.ToString()
                , dateTime.Day.ToString());
            //linux环境目录为/{1}/
            string path = string.Format(@"{0}/TempFile/{1}", "/home/www", datePath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string savePath = path + fileName;
            datePath = datePath + fileName;
            try
            {              
                using (FileStream fs = System.IO.File.Create(savePath))
                {
                    imgFile.CopyTo(fs);
                    fs.Flush();
                }
                return new JsonResult(new { savepath = savePath,datepath= datePath, code = 200 });
            }
            catch (Exception e)
            {
                return new JsonResult(new {message=e.Message,code=500});
            }
        }
        [HttpDelete("{year}/{month}/{day}/{fileName}/{extension}")]
        public JsonResult DeleteFile(string year,string month,string day,string fileName,string extension)
        {
            string datePath = string.Format("{0}/{1}/{2}/{3}.{4}", year, month, day,fileName,extension);
            string path = string.Format(@"{0}/TempFile/{1}", "/home/www", datePath);           
            if (!Directory.Exists(path))
                return new JsonResult(new { message = "不存在目录："+path, code = 500 });
            System.IO.File.Delete(path);
            return new JsonResult(new { code = 200 });
        }
    }
}
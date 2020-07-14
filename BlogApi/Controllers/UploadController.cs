using System;
using Microsoft.AspNetCore.Mvc;
using System.IO;
namespace BlogApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private string http = "http://58.87.92.221";
        //private string http = "D:";
        [HttpGet("{path}")]
        public string TestGet(string path)
        {
            return path;
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("upload/image")]
        public IActionResult Upload()
        {
            var imgFile = Request.Form.Files[0];
            string fileName = imgFile.FileName;
            string url;
            string newFileName;
            UploadFile(imgFile, fileName, out url,out newFileName);
            return new JsonResult(new { uploaded = 1, fileName = newFileName, url = url });

        }

        private  IActionResult UploadFile(Microsoft.AspNetCore.Http.IFormFile imgFile, string fileName,out string url,out string newName)
        {
            int index = fileName.LastIndexOf('.');
            string extension = fileName.Substring(index, fileName.Length - index);//获取后缀名
            string guid = Guid.NewGuid().ToString().Replace("-", "");//生成guid
            string newFileName = guid + extension;
            newName = newFileName;
            DateTime dateTime = DateTime.Now;
            //路径日期部分
            string datePath = string.Format("{0}/{1}/{2}/", dateTime.Year.ToString(), dateTime.Month.ToString()
                , dateTime.Day.ToString());
            //linux环境目录为/{1}/
            string path = string.Format(@"/{0}/TempFile/{1}", "home/www", datePath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            datePath = datePath + newFileName;
            try
            {
                using (FileStream fs = System.IO.File.Create(path + newFileName))
                {
                    imgFile.CopyTo(fs);
                    fs.Flush();
                }
                string savePath =string.Format("{0}/bf/{1}",http,datePath);
                url = savePath;
                return new JsonResult(new { savepath = savePath, datepath = datePath, code = '0',length=imgFile.Length });
            }
            catch (Exception e)
            {
                url = "";
                return new JsonResult(new { message = e.Message, code = '1' });
            }
        }

        /// <summary>
        /// 上传视频
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("upload/video")]
        [RequestSizeLimit(100_000_000)]//最大100M
        public  IActionResult UploadVideo()
        {
            try
            {
                string description = Request.Form["description"];
                string lable = Request.Form["lable"];
                if (string.IsNullOrEmpty(description))
                    return new JsonResult(new { code = "1", msg = "描述为空" });
                var imgFile = Request.Form.Files[0];
                string url;
                string newFileName;
                return UploadFile(imgFile, imgFile.FileName,out url,out newFileName);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = "1", message = ex.Message });
            }

        }
        [HttpDelete]
        [Route("upload/delete/{year}/{month}/{day}/{fileName}/{extension}")]
        public JsonResult DeleteFile(string year,string month,string day,string fileName,string extension)
        {
            string datePath = string.Format("{0}/{1}/{2}/{3}.{4}", year, month, day,fileName,extension);
            string path = string.Format(@"{0}/TempFile/{1}", "/home/www", datePath);           
            if (!System.IO.File.Exists(path))
                return new JsonResult(new { message = "不存在目录："+path, code = 500 });
            System.IO.File.Delete(path);
            return new JsonResult(new { code = 200 });
        }
    }
}
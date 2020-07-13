﻿using System;
using Microsoft.AspNetCore.Mvc;
using System.IO;
namespace BlogApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private string http = "http://127.0.0.1";
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
            return UploadFile(imgFile, fileName);
        }

        private  IActionResult UploadFile(Microsoft.AspNetCore.Http.IFormFile imgFile, string fileName)
        {
            int index = fileName.LastIndexOf('.');
            string extension = fileName.Substring(index, fileName.Length - index);//获取后缀名
            string guid = Guid.NewGuid().ToString().Replace("-", "");//生成guid
            string newFileName = guid + extension;
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
                string savePath =http+ path + newFileName;
                return new JsonResult(new { savepath = savePath, datepath = datePath, code = '0',length=imgFile.Length });
            }
            catch (Exception e)
            {
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
                return UploadFile(imgFile, imgFile.FileName);
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
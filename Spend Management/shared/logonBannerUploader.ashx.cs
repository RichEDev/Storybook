using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Spend_Management.shared
{
    /// <summary>
    /// Handler for file uploading services
    /// </summary>
    public class logonBannerUploader : IHttpHandler
    {
        /// <summary>
        /// Process request from the form
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string dirFullPath = HttpContext.Current.Server.MapPath("~/shared/images/logonimages/");
            string[] files;
            int numFiles;
            files = System.IO.Directory.GetFiles(dirFullPath);
            numFiles = files.Length + 1;
            string currentImage = "";

            foreach (string s in context.Request.Files)
            {
                HttpPostedFile file = context.Request.Files[s];
                string fileName = file.FileName;
                string fileExtension = file.ContentType;

                if (!string.IsNullOrEmpty(fileName))
                {
                    fileExtension = Path.GetExtension(fileName);
                    currentImage = Guid.NewGuid().ToString() + fileExtension;
                    string pathToSave = HttpContext.Current.Server.MapPath("~/shared/images/logonimages/") + currentImage;
                    file.SaveAs(pathToSave);
                }
            }
            context.Response.Write(currentImage);

        }

        /// <summary>
        /// Disable reusable action
        /// </summary>
        public bool IsReusable => false;
    }
}
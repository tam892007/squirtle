using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace BSE365.Helper
{
    public static class FileHelper
    {
        public static async Task<string> Upload(HttpRequestMessage request, string username)
        {
            var uploadRelativeDirectory = string.Format(@"~/Uploads/{0}/{1:yyyyMMdd}/", username, DateTime.Now);
            var uploadAbsoluteDirectory = System.Web.Hosting.HostingEnvironment.MapPath(uploadRelativeDirectory);

            var uploadAbsoluteDirectoryInfo = new DirectoryInfo(uploadAbsoluteDirectory);
            uploadAbsoluteDirectoryInfo.Create();
            var streamProvider = new MultipartFormDataStreamProvider(
                uploadAbsoluteDirectory);
            //HttpContext.Current.Server.MapPath("~/Uploads/"));
            var returnProvider = await request.Content.ReadAsMultipartAsync(streamProvider);
            // uploadedFileInfo object will give you some additional stuff like file length,
            // creation time, directory name, a few filesystem methods etc..
            var fileData = returnProvider.FileData.First();
            var uploadedFileInfo = new FileInfo(fileData.LocalFileName);
            // On upload, files are given a generic name like "BodyPart_26d6abe1-3ae1-416a-9429-b35f15e6e5d5"
            // so this is how you can get the original file name
            var originalFileName = GetDeserializedFileName(fileData);
            // Remove this line as well as GetFormData method if you're not

            // Save as orginal name
            var newFileName = string.Format(@"{1:hhmmss}_{0}",
                originalFileName,
                DateTime.Now);
            var newFileAbsoluteUrl = string.Format(@"{0}{1}",
                uploadAbsoluteDirectory,
                newFileName);
            uploadedFileInfo.MoveTo(newFileAbsoluteUrl);
            var newFileRelativeUrl = string.Format(@"{0}{1}",
                uploadRelativeDirectory,
                newFileName);

            var result = newFileRelativeUrl;
            return result;
        }

        private static string GetDeserializedFileName(MultipartFileData fileData)
        {
            var fileName = GetFileName(fileData);
            return JsonConvert.DeserializeObject(fileName).ToString();
        }

        private static string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }
    }
}
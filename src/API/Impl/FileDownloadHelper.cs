using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Api.Impl
{
    public class FileDownloadHelper
    {
        public FileDownloadHelper()
        {
        }
        public HttpResponseMessage ReturnResponse(ApiController contrller, string file, bool deleteOriginalFile = false, bool isImageFile = false)
        {
            var responseStream = new MemoryStream();
            Stream fileStream = File.Open(file, FileMode.Open);
            fileStream.CopyTo(responseStream);
            fileStream.Close();

            responseStream.Position = 0;

            var response = contrller.Request.CreateResponse();
            response.Content = new StreamContent(responseStream);

            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Octet);
            if (isImageFile)
            {
                response.Content.Headers.Add("FileResponseType", "Image");
                HttpContext.Current.Response.AddHeader("Access-Control-Expose-Headers", "FileResponseType");
            }
         
            if (!deleteOriginalFile) return response;

            try
            {
                File.Delete(file);
            }
            catch (Exception ex)
            {

            }
            return response;

        }
        public HttpResponseMessage ReturnExcelResponseFromHtml(ApiController contrller, string fileHtml)
        {
            var responseStream = new MemoryStream();
            var writer = new StreamWriter(responseStream);
            writer.Write(fileHtml);
            writer.Flush();

            responseStream.Position = 0;

            var response = contrller.Request.CreateResponse();
            response.Content = new StreamContent(responseStream);

            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            return response;
        }

        public HttpResponseMessage ReturnExcelResponseFromExcel(ApiController contrller, string filePath, bool deleteFile = false)
        {
            var responseStream = new MemoryStream();
            Stream fileStream = File.Open(filePath, FileMode.Open);
            fileStream.CopyTo(responseStream);
            fileStream.Close();

            responseStream.Position = 0;

            var response = contrller.Request.CreateResponse();
            response.Content = new StreamContent(responseStream);

            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            if (!deleteFile) return response;

            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public HttpResponseMessage ReturnResponseForEvidence(ApiController contrller, string file, bool deleteOriginalFile = false, string fileName="")
        {
            var responseStream = new MemoryStream();
            Stream fileStream = File.Open(file, FileMode.Open);
            fileStream.CopyTo(responseStream);
            fileStream.Close();

            responseStream.Position = 0;

            var response = contrller.Request.CreateResponse();
            response.Content = new StreamContent(responseStream);

            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Octet);
            if (!string.IsNullOrEmpty(fileName))
            {
                response.Content.Headers.Add("FileName", fileName);
                HttpContext.Current.Response.AddHeader("Access-Control-Expose-Headers", "FileName");
            }

            if (!deleteOriginalFile) return response;

            try
            {
                File.Delete(file);
            }
            catch (Exception ex)
            {

            }
            return response;

        }
    }
}
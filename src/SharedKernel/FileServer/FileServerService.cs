using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharedKernel.FileServer
{
    public class FileServerService
    {
        private string _fileServerUrl;

        public FileServerService(string fileServerUrl)
        {
            _fileServerUrl = fileServerUrl;
        }

        public byte[] Download(Guid id)
        {
            var client = new RestClient(_fileServerUrl);
            var request = new RestRequest($"Image/{id}");
            return client.DownloadData(request);
        }


        public Guid Upload(Stream file)
        {
            var memoryStream = new MemoryStream();
            var client = new RestClient(_fileServerUrl);
            var request = new RestRequest("Image");

            file.CopyTo(memoryStream);

            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddFileBytes("file", memoryStream.ToArray(), "image", "image/jpeg");

            IRestResponse<object> result = client.Post<object>(request);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return new Guid(result.Data.ToString());
            else if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                throw new Exception((string)result.Data);
            else
                throw new Exception("Upload file fail. " + result.ErrorMessage);
        }

        public void Update(Stream file, Guid id)
        {
            var memoryStream = new MemoryStream();
            var client = new RestClient(_fileServerUrl);
            var request = new RestRequest("Image/" + id.ToString());

            file.CopyTo(memoryStream);

            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddFileBytes("file", memoryStream.ToArray(), "image", "image/jpeg");

            IRestResponse<object> result = client.Put<object>(request);
            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                throw new Exception((string)result.Data);
            else if(result.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("Upload file fail.");
        }
    }
}

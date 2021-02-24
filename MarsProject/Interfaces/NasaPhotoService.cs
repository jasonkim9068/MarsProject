using MarsProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace MarsProject.Interfaces
{
    public class NasaPhotoService : INasaPhotosService
    {
        private readonly ILogger<NasaPhotoService> _logger;

        public NasaPhotoService(ILogger<NasaPhotoService> log)
        {
            _logger = log;
        }

        public List<RequestDate> GetValidDate(IFormFile file)
        {
            var requestDates = new List<RequestDate>();
            try
            {
                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                        requestDates.Add(new RequestDate()
                        {
                            Date = reader.ReadLine()
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking {file.FileName}.txt ", file.FileName);
            }
            return requestDates;
        }

        public void SaveImage(List<Photo> photos)
        {
            if(photos.Count > 0)
            foreach (Photo image in photos)
                using (WebClient sendData = new WebClient())
                {
                    try
                    {
                        var fileName = image.Earth_date+ "_" + image.Id.ToString();
                        //need to have folder at C:/app/Images
                        sendData.DownloadFileTaskAsync(new Uri(image.Img_src), "Images//" + fileName + ".jpg");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occur while downloading Image ID {image.Id}. ", image.Id);

                    }
                }
        }

        public void SumResult(List<Photo> photos, HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            //deserialize object
            var jsonList = JsonConvert.DeserializeObject<JsonObject>(content);
            photos.AddRange(jsonList.Photos);
        }
    }
}

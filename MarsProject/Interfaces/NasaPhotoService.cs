using MarsProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace MarsProject.Interfaces
{
    public class NasaPhotoService : INasaPhotosService
    {
        private readonly ILogger<NasaPhotoService> _logger;
        private readonly IConfiguration _appSettings;

        public NasaPhotoService(ILogger<NasaPhotoService> log, IConfiguration appSettings)
        {
            _logger = log;
            _appSettings = appSettings;
        }

        public void GetList(List<RequestDate> list, List<Photo> photos)
        {
            var formats = CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns().ToList();
            formats.Add("MMMM dd, yyyy");
            formats.Add("MMM-dd-yyyy");

            foreach (var request in list)
            {
                if (TryParseExact(request, formats, out var dateVal))
                {
                    //get API address
                    var apiUrl =
                        $"{_appSettings["BaseUrl"]}?api_key={_appSettings["APIKey"]}&earth_date={dateVal.Year}-{dateVal.Month}-{dateVal.Day}";

                    using var response = new HttpClient { }.GetAsync(apiUrl).Result;
                    response.EnsureSuccessStatusCode();

                    //if API response success then add to array
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        SumResult(photos, response);
                    }
                }
            }
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

        public bool TryParseExact(RequestDate request, List<string> formats, out DateTime dateVal)
        {
            var date = DateTime.TryParseExact(request.Date, formats.ToArray(), CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dateVal);
            
            return date;
        }

        void SumResult(List<Photo> photos, HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            //deserialize object
            var jsonList = JsonConvert.DeserializeObject<JsonObject>(content);
            photos.AddRange(jsonList.Photos);
        }
    }
}

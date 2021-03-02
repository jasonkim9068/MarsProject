using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MarsProject.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using MarsProject.Interfaces;

namespace MarsProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly INasaPhotosService _nasaPhotoService;
        private readonly IConfiguration _appSettings;
        private readonly ILogger _logger;

        public HomeController(INasaPhotosService nasaPhotoService, IConfiguration appSettings, ILogger<HomeController> log)
        {
            _logger = log;
            _nasaPhotoService = nasaPhotoService;
            _appSettings = appSettings;

        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("Demo")]
        public IActionResult Demo()
        {
            return View();
        }

        public JsonResult GetFormData(IFormFile file)
        {
            var photos = new List<Photo>();
            var message = new StringBuilder();

            //get valid dates from text file
            List<RequestDate> requestDates = _nasaPhotoService.GetValidDate(file);

            //add all possilbe date format
            var formats = CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns().ToList();
            formats.Add("MMMM dd, yyyy");
            formats.Add("MMM-dd-yyyy");

            try
            {
                if (requestDates.Count > 0)
                    foreach (var request in requestDates)
                    {   
                        if (DateTime.TryParseExact(request.Date, formats.ToArray(), CultureInfo.InvariantCulture,
                                              DateTimeStyles.None, out DateTime dateVal))
                        {
                            //get API address
                            var apiUrl = $"{_appSettings["BaseUrl"]}?api_key={_appSettings["APIKey"]}&earth_date={dateVal.Year}-{dateVal.Month}-{dateVal.Day}";

                            using var response = new HttpClient { }.GetAsync(apiUrl).Result;
                            response.EnsureSuccessStatusCode();

                            //if API response success then add to array
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                _nasaPhotoService.SumResult(photos, response);
                            }
                        }
                    }
                else
                {
                    message.AppendFormat("No records exists");
                    _logger.LogError("No records exists");

                    return Json(new { success = false, data = photos, message = message.ToString() });

                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unknown error occurred .");
                return Json(new { success = false, data = photos, message = message.ToString() });
            }

            if (photos.Count > 0)
            // save all image to local path (C:\Users\Jason Kim\source\repos\MarsProject\MarsProject\Images)
            _nasaPhotoService.SaveImage(photos);

            return Json(new { success = true, data = photos });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

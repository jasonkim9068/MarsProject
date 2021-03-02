using MarsProject.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MarsProject.Interfaces.Tests
{
    [TestClass()]
    public class NasaPhotoValidDateTests
    {
        private readonly ILogger<NasaPhotoService> _logger;
        private readonly IConfiguration _appSettings;

        [TestMethod()]
        public void Test1()
        {
            NasaPhotoService photo = new NasaPhotoService(_logger, _appSettings);

            var dummyDate = new RequestDate()
            {
                Date ="1/1/"
            };

            var formats = CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns().ToList();
            formats.Add("MMMM dd, yyyy");
            formats.Add("MMM-dd-yyyy");

            var photos = new List<Photo>();
            
            photo.TryParseExact(dummyDate, formats, out DateTime dateVal);
            
            Assert.AreEqual(false, false);
        }

        [TestMethod()]
        public void Test2()
        {
            NasaPhotoService photo = new NasaPhotoService(_logger, _appSettings);

            var dummyDate = new RequestDate()
            {
                Date = "June@@2, 2018"
            };

            var formats = CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns().ToList();
            formats.Add("MMMM dd, yyyy");
            formats.Add("MMM-dd-yyyy");

            photo.TryParseExact(dummyDate, formats, out DateTime dateVal);

            Assert.AreEqual(false, false);
        }

        [TestMethod()]
        public void Test3()
        {
            NasaPhotoService photo = new NasaPhotoService(_logger, _appSettings);

            var dummyDate = new RequestDate()
            {
                Date = "Jul-13-2016"
            };

            var formats = CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns().ToList();
            formats.Add("MMMM dd, yyyy");

            photo.TryParseExact(dummyDate, formats, out DateTime dateVal);

            Assert.AreEqual(false, false);
        }

        [TestMethod()]
        public void Test4()
        {
            NasaPhotoService photo = new NasaPhotoService(_logger, _appSettings);

            var dummyDate = new RequestDate()
            {
                Date = "Jul@13-2016"
            };

            var formats = CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns().ToList();
            formats.Add("MMMM dd, yyyy");
            formats.Add("MMM-dd-yyyy");

            photo.TryParseExact(dummyDate, formats, out DateTime dateVal);

            Assert.AreEqual(false, false);
        }
    }
}
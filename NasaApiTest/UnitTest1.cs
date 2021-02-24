using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using MarsProject.Controllers;
using MarsProject.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using Moq;

namespace NasaApiTest
{
    public class Tests
    {
        private IConfiguration _configMock;
        private INasaPhotosService _nasaPhotoService;
        private ILogger<HomeController> _logger;

        [SetUp]
        public void Setup()
        {
        }

        // Test with dummy text file
        [Test]
        public void Test1()
        {
            _configMock = InitConfiguration();
            HomeController nasaObj = new HomeController(_nasaPhotoService, _configMock, _logger);
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = fileMock.Object;

            JsonResult GetFormData = nasaObj.GetFormData(file);
            Assert.Fail("success", GetFormData);
        }

        [Test]
        public void Test2()
        {
            _configMock = InitConfiguration();
            HomeController nasaObj = new HomeController(_nasaPhotoService, _configMock, _logger);
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("02/27/17")), 0, 0, "Data", "dummy.txt");

            JsonResult GetFormData = nasaObj.GetFormData(file);
            Assert.Fail("success", GetFormData);
        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            return config;
        }
    }
}
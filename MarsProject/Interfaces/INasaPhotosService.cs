using MarsProject.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MarsProject.Interfaces
{
    public interface INasaPhotosService
    {
        List<RequestDate> GetValidDate(IFormFile file);
        void GetList(List<RequestDate> list, List<Photo> photos);

        void SaveImage(List<Photo> photos);
    }
}

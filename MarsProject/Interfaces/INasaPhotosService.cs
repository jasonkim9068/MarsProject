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

        void SaveImage(List<Photo> photos);
        void SumResult(List<Photo> photos, HttpResponseMessage response);
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IImageService
    {
        public Task<string> SaveImageAsync(IFormFile imageFile);
        public void DeleteImage(string imageNameWithExtension);
        public (FileStream,string) GetImageStream(string imageNameWithExtension);
    }
}

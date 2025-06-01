using Microsoft.AspNetCore.Http;

namespace Services.DTOs
{
    public class ImageDTO
    {
        public IFormFile imageFile { get; set; }
    }
}

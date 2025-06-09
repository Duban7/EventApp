using Microsoft.AspNetCore.Http;

namespace Services.Interfaces
{
    public interface IImageService
    {
        public Task<string> SaveImageAsync(IFormFile imageFile, CancellationToken cancellationToken = default);
        public void DeleteImage(string imageNameWithExtension);
        public (FileStream,string) GetImageStream(string imageNameWithExtension);
    }
}

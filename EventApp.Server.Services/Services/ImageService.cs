using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Services.Exeptions;
using Services.Interfaces;

namespace Services.Services
{
    public class ImageService : IImageService
    {
        private readonly string[] allowedFileExtensions = [".png", ".jpg", ".jpeg"];
        private readonly string rootPath;
        public ImageService(IWebHostEnvironment environment)
        {
            rootPath = environment.ContentRootPath;
        }
        public async Task<string> SaveImageAsync(IFormFile imageFile, CancellationToken cancellationToken)
        {
            if(imageFile == null)
                throw new BadRequestException("Image wasn't sent");

            string path = Path.Combine(rootPath, "Uploads", "EventsImages");
            if(!Directory.Exists(path)) 
                Directory.CreateDirectory(path);

            var ext = Path.GetExtension(imageFile.FileName);
            if (!allowedFileExtensions.Contains(ext))
                throw new BadRequestException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");


            var fileName = $"{Guid.NewGuid().ToString()}{ext}";
            var fileNameWithPath = Path.Combine(path, fileName);
            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await imageFile.CopyToAsync(stream, cancellationToken);

            return fileName;
        }
        public void DeleteImage(string imageNameWithExtension)
        {
            if (string.IsNullOrEmpty(imageNameWithExtension))
                throw new BadRequestException("Image id wasn't sent");

            var path = Path.Combine(rootPath, "Uploads", "EventsImages", imageNameWithExtension);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Invalid file path");

            File.Delete(path);
        }

        public (FileStream,string) GetImageStream(string imageNameWithExtension)
        {
            string path = Path.Combine(rootPath, "Uploads", "EventsImages", imageNameWithExtension);
            if (!File.Exists(path))
                throw new BadRequestException("image not found");

            var imageFileStream = File.OpenRead(path);
            var ext = Path.GetExtension(imageNameWithExtension);

            return (imageFileStream,ext);
        }

    }
}

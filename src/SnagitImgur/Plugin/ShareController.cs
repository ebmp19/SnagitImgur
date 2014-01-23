using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SnagitImgur.Plugin.ImageService;
using SNAGITLib;

namespace SnagitImgur.Plugin
{
    public class ShareController
    {
        private readonly ISnagIt snagitHost;
        private readonly ISnagItAsyncOutput asyncOutput;

        public ShareController(ISnagIt snagitHost)
        {
            this.snagitHost = snagitHost;
            asyncOutput = snagitHost as ISnagItAsyncOutput;
        }

        public async Task ShareImage(IImageService service)
        {
            string imagePath = GetCapturedImage();
            try
            {
                ImageInfo result = await service.UploadAsync(imagePath);
                
                Process.Start(result.Url);
            }
            finally
            {
                File.Delete(imagePath);
            }
        }

        private string GetCapturedImage()
        {
            ISnagItDocument snagItDocument = snagitHost.SelectedDocument;
            var imageDocumentSave = snagItDocument as ISnagItImageDocumentSave;
            if (imageDocumentSave == null)
            {
                throw new InvalidCastException("Unable to get image saving facility of Snagit");
            }

            string tempFileName = Path.GetTempFileName() + ".png";
            imageDocumentSave.SaveToFile(ref tempFileName, snagImageFileType.siftPNG, null);

            return tempFileName;
        }
    }
}
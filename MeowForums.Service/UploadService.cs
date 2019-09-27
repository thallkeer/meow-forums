using MeowForums.Data.DataInterfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using Dropbox.Api;
using Dropbox.Api.Files;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
using System.Diagnostics;
using Imgur.API;
using Imgur.API.Models.Impl;
using System.Drawing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using System.IO;
using SixLabors.ImageSharp.Formats;

namespace MeowForums.Service
{
    public class UploadService : IUploadService
    {
        public CloudBlobContainer GetBlobContainer(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference("profile-images");
        }

        /// <summary>
        /// Method uploads the form file to the cloud and return its shared link
        /// </summary>
        /// <param name="file">File to upload</param>
        /// <param name="folder">Directory under which file will be uploaded</param>
        /// <returns></returns>
        public async Task<Uri> UploadImage(IFormFile file, string folder)
        {
            using (var client = GetClient())
            {
                using (var mem = file.OpenReadStream())
                {
                    var path = "/" + folder + "/" + file.FileName;
                    var updated = await client.Files.UploadAsync(
                        path,
                        WriteMode.Overwrite.Instance,
                        body: mem);

                    return GetImageUri(client, path, updated.Name);
                }
            }
        }

        private Uri GetImageUri(DropboxClient client, string path, string mdName)
        {
            var sharedLinks = client.Sharing.ListSharedLinksAsync(path).Result;
            var myLink = sharedLinks.Links.FirstOrDefault(l => l.Name == mdName);

            if (myLink == null)
                myLink = client.Sharing.CreateSharedLinkWithSettingsAsync(path).Result;

            return new Uri(myLink.Url.Replace("dl=0", "raw=1")); ;
        }

        private DropboxClient GetClient()
        {
            return new DropboxClient("ySjDKy2DFMAAAAAAAAAG0intvO7_0-jtUto8IHikaJ47lcHVDXyMXu6dizI3wHdF", new DropboxClientConfig("MeowForums"));
        }

        private const string CLIENT_ID = "357e279f5d0933b";
        private const string CLIENT_SECRET = "6dc9ad1f9deac4d80d19272fee8072b511037472";

        //public OAuth2Token CreateToken()
        //{
        //    var token = new OAuth2Token(TOKEN_ACCESS, REFRESH_TOKEN, TOKEN_TYPE, ID_ACCOUNT, IMGUR_USER_ACCOUNT, int.Parse(EXPIRES_IN));
        //    return token;
        //}

        //Use it only if your token is expired
        //public Task<IOAuth2Token> RefreshToken()
        //{
        //    var client = new ImgurClient(CLIENT_ID, CLIENT_SECRET);
        //    var endpoint = new OAuth2Endpoint(client);
        //    var token = endpoint.GetTokenByRefreshTokenAsync(REFRESH_TOKEN);
        //    return token;
        //}

        public async Task<Uri> UploadImage(IFormFile file)
        {
            try
            {
                var client = new ImgurClient(CLIENT_ID, CLIENT_SECRET);
                var endpoint = new ImageEndpoint(client);
                Imgur.API.Models.IImage image;
                //Here you have to link your image location
                //using (var fs = new FileStream(@"E:\repos\MeowForums\MeowForums\wwwroot\images\forum-logo-s.png", FileMode.Open))
                //{
                //    image = await endpoint.UploadImageStreamAsync(fs);
                //}
                using (var mem = file.OpenReadStream())
                {
                    //using (var mem2 = new MemoryStream())
                    //{
                    //    using (var img = SixLabors.ImageSharp.Image.Load(mem, out IImageFormat format))
                    //    {
                    //        img.Mutate(i => i.Resize
                    //        (
                    //            new ResizeOptions
                    //            {
                    //                Size = new SixLabors.Primitives.Size(140, 140),
                    //                Mode = ResizeMode.Crop
                    //            }
                    //        ));

                    //        img.Save(mem2, format);
                    //        image = await endpoint.UploadImageStreamAsync(mem2);
                    //    }
                    //}
                    image = await endpoint.UploadImageStreamAsync(mem);
                }
                Debug.Write("Image uploaded. Image Url: " + image.Link);
                return new Uri(image.Link);                
            }
            catch (ImgurException imgurEx)
            {
                Debug.Write("Error uploading the image to Imgur");
                Debug.Write(imgurEx.Message);
            }
            return new Uri(string.Empty);
        }



        //private static Image resizeImage(Image imgToResize, Size size)
        //{
        //    int sourceWidth = imgToResize.Width;
        //    int sourceHeight = imgToResize.Height;

        //    float nPercent = 0;
        //    float nPercentW = 0;
        //    float nPercentH = 0;

        //    nPercentW = ((float)size.Width / (float)sourceWidth);
        //    nPercentH = ((float)size.Height / (float)sourceHeight);

        //    if (nPercentH < nPercentW)
        //        nPercent = nPercentH;
        //    else
        //        nPercent = nPercentW;

        //    int destWidth = (int)(sourceWidth * nPercent);
        //    int destHeight = (int)(sourceHeight * nPercent);

        //    //Bitmap b = new Bitmap(destWidth, destHeight);
        //    //Graphics g = Graphics.FromImage((Image)b);
        //    //g.InterpolationMode = InterpolationMode.HighQualityBicubic;

        //    g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
        //    g.Dispose();

        //    return (Image)b;
        //}
    }
}

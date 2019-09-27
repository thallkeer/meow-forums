using MeowForums.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MeowForums.Data.DataInterfaces
{
    public interface IUploadService
    {
        CloudBlobContainer GetBlobContainer(string connectionString);
        Task<Uri> UploadImage(IFormFile file, string folder);
        Task<Uri> UploadImage(IFormFile file);
    }
}

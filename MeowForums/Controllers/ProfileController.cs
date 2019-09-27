using MeowForums.Data.DataInterfaces;
using MeowForums.Data.Models;
using MeowForums.Models.AppUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MeowForums.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IApplicationUserService userService;
        private readonly IUploadService uploadService;
        private readonly IConfiguration configuration;

        public ProfileController(UserManager<ApplicationUser> userManager, 
            IApplicationUserService userService,
            IUploadService uploadService,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.userService = userService;
            this.uploadService = uploadService;
            this.configuration = configuration;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var profiles = userService.GetAll()
                                      .OrderByDescending(user => user.Rating)
                                      .Select(u => new ProfileModel
                                      {
                                          Email = u.Email,
                                          UserId = u.Id,
                                          UserName = u.UserName,
                                          ProfileImageUrl = u.ProfileImageUrl,
                                          UserRating = u.Rating.ToString(),
                                          MemberSince = u.MemberSince
                                      });
            var model = new ProfileListModel
            {
                Profiles = profiles
            };
            return View(model);
        }

        public async Task SeedUsers()
        {
            var user = new ApplicationUser { UserName = "Ryan", MemberSince = DateTime.Now };
            await userManager.CreateAsync(user, "Test-1234");
            var user1 = new ApplicationUser { UserName = "Randall", MemberSince = DateTime.Now };
            await userManager.CreateAsync(user1, "Test-1234");
            var user2 = new ApplicationUser { UserName = "Diana", MemberSince = DateTime.Now };
            await userManager.CreateAsync(user2, "Test-1234");
            var user3 = new ApplicationUser { UserName = "Linda", MemberSince = DateTime.Now };
            await userManager.CreateAsync(user3, "Test-1234");
            var user4 = new ApplicationUser { UserName = "Kit", MemberSince = DateTime.Now };
            await userManager.CreateAsync(user4, "Test-1234");
            var user5 = new ApplicationUser { UserName = "Sophie", MemberSince = DateTime.Now };
            await userManager.CreateAsync(user5, "Test-1234");

        }

        public IActionResult Detail(string id)
        {
            var user = userService.GetById(id);
            var userRoles = userManager.GetRolesAsync(user).Result;

            var model = new ProfileModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserRating = user.Rating.ToString(),
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                MemberSince = user.MemberSince,
                IsAdmin = userRoles.Contains("Admin")
            };
            return View(model);
        }

        //[HttpPost]
        //public async Task<IActionResult> UploadProfileImage(IFormFile file)
        //{
        //    var userId = userManager.GetUserId(User);
        //    //Connect to an Azure Storage Container
        //    var connectionString = configuration.GetConnectionString("AzureStorageAccount");
        //    //Get Blob Container
        //    var container = uploadService.GetBlobContainer(connectionString);
        //    //Parse the Content Disposition response header\
        //    var contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
        //    //Grab the filename
        //    var filename = contentDisposition.FileName.Trim('"');
        //    //get a reference to a Block Blob
        //    var blockBlob = container.GetBlockBlobReference(filename);
        //    //On that block block, upload our file <-- file uploaded to hte cloud
        //    await blockBlob.UploadFromStreamAsync(file.OpenReadStream());
        //    //set the user's profile image to the URI\
        //    await userService.SetProfileImage(userId, blockBlob.Uri);
        //    //Redirect to the user's profile page
        //    return RedirectToAction("Detail", "Profile", new { id = userId });
        //}

        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var userId = userManager.GetUserId(User);
            var user = userService.GetById(userId);

            Uri uri = await uploadService.UploadImage(file);
            /*await uploadService.UploadImage(file, $"UserImages/{user.UserName}");*/
            await userService.SetProfileImage(userId, uri);

            return RedirectToAction("Detail", "Profile", new { id = userId });
        }
    }
}
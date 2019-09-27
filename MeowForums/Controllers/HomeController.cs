using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MeowForums.Models;
using MeowForums.Models.Home;
using MeowForums.Data.DataInterfaces;
using MeowForums.Models.Post;
using MeowForums.Data.Models;

namespace MeowForums.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostService postService;
        private readonly IForumService forumService;

        public HomeController(IPostService postService, IForumService forumService)
        {
            this.postService = postService;
            this.forumService = forumService;
        }

        public IActionResult Index()
        {
            var model = BuildHomeIndexModel();
            return View(model);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private HomeIndexModel BuildHomeIndexModel()
        {            
            var latestPosts = postService.GetLatestPosts(10);            

            var posts = latestPosts.Select(post => new PostListingModel
            {
                Id = post.Id,
                Title = post.Title,
                AuthorId = post.User.Id,
                AuthorName = post.User.UserName,
                AuthorRating = post.User.Rating,
                DatePosted = post.Created.ToString(),
                Forum = GetForumListingForPost(post),
                RepliesCount = post.Replies.Count(),
                AuthorImageUrl = post.User.ProfileImageUrl
            });

            return new HomeIndexModel
            {
                LatestPosts = posts,
                SearchQuery = ""                
            };
        }       
        private ForumListingModel GetForumListingForPost(Post post)
        {
            var forum = post.Forum;

            return new ForumListingModel
            {
                Id = forum.Id,
                Name = forum.Title,
                ImageUrl = forum.ImageUrl               
            };
        } 
    }
}

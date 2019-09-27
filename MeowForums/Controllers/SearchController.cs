using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeowForums.Data.DataInterfaces;
using MeowForums.Data.Models;
using MeowForums.Models;
using MeowForums.Models.Post;
using MeowForums.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace MeowForums.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPostService postService;
        private readonly IForumService forumService;

        public SearchController(IForumService forumService, IPostService postService)
        {
            this.forumService = forumService;
            this.postService = postService;
        }
        public IActionResult Results(string searchQuery)
        {
            var posts = postService.GetFilteredPosts(searchQuery);

            bool areNoResults = (!String.IsNullOrEmpty(searchQuery) &&
                !posts.Any());

            var postListings = posts.Select(post => new PostListingModel
            {
                Id = post.Id,
                AuthorId = post.User.Id,
                AuthorName = post.User.UserName,
                AuthorImageUrl = post.User.ProfileImageUrl,
                AuthorRating = post.User.Rating,
                Title = post.Title,
                DatePosted = post.Created.ToString(),
                RepliesCount = post.Replies.Count(),
                Forum = BuildForumListing(post)
            });

            var model = new SearchResultModel
            {
                Posts = postListings,
                SearchQuery = searchQuery,
                EmptyResults = areNoResults
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult Search(string searchQuery)
        {
            return RedirectToAction("Results", new { searchQuery });
        }

        private ForumListingModel BuildForumListing(Post post)
        {
            var forum = post.Forum;
            return new ForumListingModel
            {
                Id = forum.Id,
                ImageUrl = forum.ImageUrl,
                Description = forum.Description,
                Name = forum.Title,
                NumberOfPosts = forum.Posts.Count()
            };
        }
    }
}
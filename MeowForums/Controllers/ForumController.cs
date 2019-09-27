using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeowForums.Data.DataInterfaces;
using MeowForums.Data.Models;
using MeowForums.Models;
using MeowForums.Models.Forum;
using MeowForums.Models.Post;
using MeowForums.Models.Reply;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeowForums.Controllers
{
    public class ForumController : Controller
    {
        private readonly IPostService postService;
        private readonly IForumService forumService;
        private readonly IUploadService uploadService;
        public ForumController(IForumService forumService, IPostService postService, IUploadService uploadService)
        {
            this.forumService = forumService;
            this.postService = postService;
            this.uploadService = uploadService;
        }
        public IActionResult Index()
        {


            var forums = forumService.GetAll().Select(
                forum => new ForumListingModel
                {
                    Id = forum.Id,
                    Name = forum.Title,
                    Description = forum.Description,
                    ImageUrl = forum.ImageUrl,
                    NumberOfPosts = forum.Posts?.Count() ?? 0,
                    NumberOfUsers = forumService.GetAllActiveUsers(forum.Id).Count(),
                    LastReply = BuildPostReply(forumService.GetLastMessage(forum.Id)),
                    HasRecentPost = forumService.HasRecentPost(forum.Id)
                });

            var model = new ForumIndexModel
            {
                ForumList = forums.OrderBy(f => f.Name)
            };

            return View(model);
        }

        public IActionResult Topic(int id, string searchQuery)
        {
            var forum = forumService.GetById(id);
            var posts = postService.GetFilteredPosts(forum, searchQuery).ToList();

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
                LastReply = BuildPostReply(post.Replies.OrderByDescending(reply => reply.Created).FirstOrDefault()),
                Forum = BuildForumListing(post)
            });            

            var model = new ForumTopicModel
            {
                Posts = postListings,
                Forum = BuildForumListing(forum)
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Search(int id, string searchQuery)
        {
            return RedirectToAction("Topic", new { id, searchQuery });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new AddForumModel();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddForum(AddForumModel model)
        {
            var imageUri = "/images/users/default.png";
            if (model.ImageUpload != null)
            {
                var uri = await uploadService.UploadImage(model.ImageUpload/*, $"Forum-Logos/{model.Title}"*/);
                imageUri = uri.AbsoluteUri;
            }
            var forum = new Forum
            {
                Title = model.Title,
                Description = model.Description,
                Created = DateTime.Now,
                ImageUrl = imageUri
            };
            await forumService.Create(forum);
            return RedirectToAction("Index", "Forum");
        }

        private PostReplyModel BuildPostReply(PostReply reply)
        {
            if (reply == null)
                return null;

            return new PostReplyModel
            {
                Id = reply.Id,
                AuthorId = reply.User.Id,
                AuthorName = reply.User.UserName,
                AuthorImageUrl = reply.User.ProfileImageUrl,
                AuthorRating = reply.User.Rating,
                Created = reply.Created,
                ReplyContent = reply.Content,
            };
        }

        private ForumListingModel BuildForumListing(Post post)
        {
            return BuildForumListing(post.Forum);
        }

        private ForumListingModel BuildForumListing(Forum forum)
        {
            return new ForumListingModel
            {
                Id = forum.Id,
                Name = forum.Title,
                Description = forum.Description,
                ImageUrl = forum.ImageUrl
            };        
        }
    }
}
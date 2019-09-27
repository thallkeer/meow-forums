using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeowForums.Data.DataInterfaces;
using MeowForums.Data.Models;
using MeowForums.Models.Post;
using MeowForums.Models.Reply;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MeowForums.Controllers
{    
    public class PostController : Controller
    {
        private readonly IPostService postService;
        private readonly IForumService forumService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IApplicationUserService userService;

        public PostController(IPostService postService, IForumService forumService, UserManager<ApplicationUser> userManager, IApplicationUserService applicationUserService)
        {
            this.postService = postService;
            this.forumService = forumService;
            this.userManager = userManager;
            this.userService = applicationUserService;
        }
        public IActionResult Index(int id)
        {
            var post = postService.GetById(id);            
            var replies = BuildPostReplies(post.Replies);

            var model = new PostIndexModel
            {
                Id = post.Id,
                Title = post.Title,
                AuthorId = post.User.Id,
                AuthorName = post.User.UserName,
                AuthorImageUrl = post.User.ProfileImageUrl,
                AuthorRating = post.User.Rating,
                Created = post.Created,
                PostContent = post.Content,
                Replies = replies,
                ForumId = post.Forum.Id,
                ForumName = post.Forum.Title,
                IsAuthorAdmin = IsAuthorAdmin(post.User)
            };
            return View(model);
        }
        [Authorize]
        public IActionResult Create(int id)
        {
            var forum = forumService.GetById(id);

            var model = new NewPostModel
            {
                ForumName = forum.Title,
                ForumId = forum.Id,
                ForumImageUrl = forum.ImageUrl,
                AuthorName = User.Identity.Name
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPost(NewPostModel postModel)
        {
            var userId = userManager.GetUserId(User);
            var user = await userManager.FindByIdAsync(userId);
            var post = BuildPost(postModel, user);
            await postService.Add(post); //Block the current thread until the current task is complete
                                         // TODO: Implement User Rating Management
            await userService.UpdateUserRating(userId, typeof(Post));
            return RedirectToAction("Index", "Post",new { id = post.Id });
        }

        private Post BuildPost(NewPostModel postModel, ApplicationUser user)
        {
            var forum = forumService.GetById(postModel.ForumId);
            return new Post
            {
                Title = postModel.Title,
                Content = postModel.Content,
                Created = DateTime.Now,
                User = user,
                Forum = forum
            };
        }

        private IEnumerable<PostReplyModel> BuildPostReplies(IEnumerable<PostReply> replies)
        {
            return replies.Select(reply => new PostReplyModel
            {
                Id = reply.Id,
                AuthorId = reply.User.Id,
                AuthorName = reply.User.UserName,
                AuthorImageUrl = reply.User.ProfileImageUrl,
                AuthorRating = reply.User.Rating,
                Created = reply.Created,
                ReplyContent = reply.Content,
                IsAuthorAdmin = IsAuthorAdmin(reply.User)                
            });
        }

        private bool IsAuthorAdmin(ApplicationUser user)
        {
            var roles = userManager.GetRolesAsync(user).Result;
            return roles.Contains("Admin");
        }
    }
}
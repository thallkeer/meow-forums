using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeowForums.Data.DataInterfaces;
using MeowForums.Data.Models;
using MeowForums.Models.Reply;
using MeowForums.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MeowForums.Controllers
{
    [Authorize]
    public class ReplyController : Controller
    {
        private readonly IPostService postService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IApplicationUserService userService;

        public ReplyController(IPostService postService, UserManager<ApplicationUser> userManager, IApplicationUserService userService)
        {
            this.postService = postService;
            this.userManager = userManager;
            this.userService = userService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Post id</param>
        /// <returns></returns>
        public async Task<IActionResult> Create(int id)
        {
            var post = postService.GetById(id);
            var userId = userManager.GetUserId(User);
            var user = await userManager.FindByIdAsync(userId);

            var model = new PostReplyModel
            {
                PostContent = post.Content,
                PostTitle = post.Title,
                PostId = post.Id,

                AuthorName = User.Identity.Name,
                AuthorImageUrl = user.ProfileImageUrl,
                AuthorId = user.Id,
                AuthorRating = user.Rating,
                IsAuthorAdmin = User.IsInRole("Admin"),

                ForumName = post.Forum.Title,
                ForumId = post.Forum.Id,
                ForumImageUrl = post.Forum.ImageUrl,

                Created = DateTime.Now
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddReply(PostReplyModel model)
        {
            var userId = userManager.GetUserId(User);
            var user = await userManager.FindByIdAsync(userId);

            var reply = BuildReply(model, user);

            await postService.AddReply(reply);
            await userService.UpdateUserRating(userId, typeof(PostReply));
            return RedirectToAction("Index", "Post", new { id = model.PostId });
        }

        private PostReply BuildReply(PostReplyModel model, ApplicationUser user)
        {
            var post = postService.GetById(model.PostId);
            return new PostReply
            {
                Post = post,
                Content = model.ReplyContent,
                Created = DateTime.Now,
                User = user
            };
        }
    }
}
using MeowForums.Data;
using MeowForums.Data.DataInterfaces;
using MeowForums.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeowForums.Service
{
    public class ForumService : IForumService
    {
        private readonly ApplicationDbContext context;
        public ForumService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task Create(Forum forum)
        {
            context.Add(forum);
            await context.SaveChangesAsync();
        }

        public Task Delete(int forumId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Forum> GetAll()
        {
            return context.Forums
                .Include(forum => forum.Posts);
        }

        public IEnumerable<ApplicationUser> GetAllActiveUsers(int forumId)
        {
            var posts = GetById(forumId).Posts;
            if (posts != null || !posts.Any())
            {
                var postUsers = posts.Select(p => p.User);
                var replyUsers = posts.SelectMany(p => p.Replies).Select(r => r.User);
                return postUsers.Union(replyUsers).Distinct();
            }
            return new List<ApplicationUser>();
        }

        public Forum GetById(int id)
        {
            var forum = context.Forums.Where(f => f.Id == id)
                .Include(f => f.Posts)
                    .ThenInclude(p => p.User)
                .Include(f => f.Posts)
                    .ThenInclude(p => p.Replies)
                        .ThenInclude(r => r.User)
                .FirstOrDefault();

            return forum;
        }

        public bool HasRecentPost(int forumId)
        {
            const int hoursAgo = 12;
            var window = DateTime.Now.AddHours(-hoursAgo);
            return GetById(forumId).Posts.Any(post => post.Created > window);
        }

        public PostReply GetLastMessage(int forumId)
        {
            var forum = GetById(forumId);

            var query = from reply in context.PostReplies
                        join post in forum.Posts
                        on reply.Post.Id equals post.Id
                        where post.Forum == forum
                        orderby reply.Created descending
                        select reply;

            return query.FirstOrDefault();
        }

        public Task UpdateForumDesctiption(int forumId, string newDescription)
        {
            throw new NotImplementedException();
        }

        public Task UpdateForumTitle(int forumId, string newTitle)
        {
            throw new NotImplementedException();
        }

       
    }
}

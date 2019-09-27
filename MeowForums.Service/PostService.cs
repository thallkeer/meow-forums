
using MeowForums.Data;
using MeowForums.Data.DataInterfaces;
using MeowForums.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowForums.Service
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext context;
        public PostService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task Add(Post post)
        {
            context.Posts.Add(post);
            await context.SaveChangesAsync();
        }

        public async Task AddReply(PostReply reply)
        {
            context.PostReplies.Add(reply);
            await context.SaveChangesAsync();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task EditPostContent(int id, string newContent)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Post> GetAll()
        {
            return context.Posts
                 .Include(post => post.User)
                 .Include(post => post.Replies)
                    .ThenInclude(reply => reply.User)
                 .Include(post => post.Forum);
        }

        public Post GetById(int id)
        {
            return context.Posts.Where(post => post.Id == id)
                 .Include(post => post.User)
                 .Include(post => post.Replies)
                    .ThenInclude(reply => reply.User)
                 .Include(post => post.Forum)
                 .FirstOrDefault();
        }

        public IEnumerable<Post> GetFilteredPosts(Forum forum, string searchQuery)
        {            
            return String.IsNullOrEmpty(searchQuery) ? 
                GetAll().Where(post => post.Forum == forum) :
                GetAll().Where(post 
                =>  post.Forum == forum 
                && (post.Title.ToUpper().Contains(searchQuery.ToUpper()) 
                || post.Content.ToUpper().Contains(searchQuery.ToUpper())));
        }

        public IEnumerable<Post> GetFilteredPosts(string searchQuery)
        {
            return GetAll().Where(post
                 => post.Title.ToUpper().Contains(searchQuery.ToUpper())
                 || post.Content.ToUpper().Contains(searchQuery.ToUpper()));
        }

        public IEnumerable<Post> GetLatestPosts(int count)
        {
            return context.Posts.OrderByDescending(post => post.Created).Take(count)
                .Include(post => post.User)
                .Include(post => post.Replies)
                    .ThenInclude(reply => reply.User)
                .Include(post => post.Forum);
        }

        public IEnumerable<Post> GetPostsByForum(int forumId)
        {
            return context.Forums.Where(forum => forum.Id == forumId)
                .FirstOrDefault()?.Posts;
        }
    }
}

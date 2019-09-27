using MeowForums.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MeowForums.Data.DataInterfaces
{
    public interface IPostService
    {
        Post GetById(int id);      
        IEnumerable<Post> GetAll();
        IEnumerable<Post> GetLatestPosts(int count);
        IEnumerable<Post> GetFilteredPosts(string searchQuery);
        IEnumerable<Post> GetFilteredPosts(Forum forum,string searchQuery);
        IEnumerable<Post> GetPostsByForum(int forumId);
        Task Add(Post post);
        Task Delete(int id);
        Task EditPostContent(int id, string newContent);
        Task AddReply(PostReply reply);
    }
}

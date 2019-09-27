using MeowForums.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MeowForums.Data.DataInterfaces
{
    public interface IForumService
    {
        Forum GetById(int id);
        IEnumerable<Forum> GetAll();
        IEnumerable<ApplicationUser> GetAllActiveUsers(int forumId);
        PostReply GetLastMessage(int forumId);
        bool HasRecentPost(int forumId);

        Task Create(Forum forum);
        Task Delete(int forumId);
        Task UpdateForumTitle(int forumId, string newTitle);
        Task UpdateForumDesctiption(int forumId, string newDescription);
    }
}

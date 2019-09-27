using MeowForums.Data;
using MeowForums.Data.DataInterfaces;
using MeowForums.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MeowForums.Service
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly ApplicationDbContext context;
        public ApplicationUserService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IEnumerable<ApplicationUser> GetAll()
        {
            return context.Users;
        }

        public ApplicationUser GetById(string id)
        {
            return context.Users.Find(id);
        }

        public async Task UpdateUserRating(string userId, Type type)
        {
            var user = GetById(userId);
            user.Rating = CalculateUserRating(type, user.Rating);
            await context.SaveChangesAsync();
        }

        private int CalculateUserRating(Type type, int userRating)
        {
            var inc = 0;
            if (type == typeof(Post))
                inc = 3;

            else if (type == typeof(PostReply))
                inc = 1;

            return userRating + inc;
        }

        public async Task SetProfileImage(string id, Uri uri)
        {
            var user = GetById(id);
            user.ProfileImageUrl = uri.AbsoluteUri;
            context.Update(user);
            await context.SaveChangesAsync();
        }        
    }
}

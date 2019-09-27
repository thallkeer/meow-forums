using MeowForums.Data;
using MeowForums.Service;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace MeowForums.Tests
{
    [TestFixture]
    public class PostService_Should
    {
        [Test]
        public void Return_Filtered_Results_Corresponding_To_Query()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Search_Database").Options;

            //Arrange
            using (var ctx = new ApplicationDbContext(options))
            {
                ctx.Forums.Add(new Data.Models.Forum
                {
                    Id = 19
                });

                ctx.Posts.Add(new Data.Models.Post
                {
                    Forum = ctx.Forums.Find(19),
                    Id = 23523,
                    Title = "First Post",
                    Content = "Coffee"
                });
                ctx.Posts.Add(new Data.Models.Post
                {
                    Forum = ctx.Forums.Find(19),
                    Id = -2144,
                    Title = "Coffee",
                    Content = "Some content"
                });
                ctx.Posts.Add(new Data.Models.Post
                {
                    Forum = ctx.Forums.Find(19),
                    Id = 223,
                    Title = "Tea",
                    Content = "Coffee"
                });

                ctx.SaveChanges();
            }
            //Act
            using (var ctx = new ApplicationDbContext(options))
            {
                var postService = new PostService(ctx);
                var result = postService.GetFilteredPosts("coffee");

                var postCount = result.Count();

                //Assert
                Assert.AreEqual(3, postCount);
            }            
        }
    }
}

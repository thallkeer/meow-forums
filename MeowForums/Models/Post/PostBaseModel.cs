using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeowForums.Models.Post
{
    public abstract class PostBaseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int AuthorRating { get; set; }
        public string AuthorImageUrl { get; set; }
    }
}

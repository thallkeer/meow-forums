using MeowForums.Models.Reply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeowForums.Models.Post
{
    public class PostIndexModel : PostBaseModel
    {         
        public DateTime Created { get; set; }
        public bool IsAuthorAdmin { get; set; }
        public string PostContent { get; set; }
        public int ForumId { get; set; }
        public string ForumName { get; set; }

        public IEnumerable<PostReplyModel> Replies { get; set; }
    }
}

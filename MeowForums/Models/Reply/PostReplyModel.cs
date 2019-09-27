using MeowForums.Models.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeowForums.Models.Reply
{
    public class PostReplyModel : PostBaseModel
    {
        public bool IsAuthorAdmin { get; set; }

        public DateTime Created { get; set; }
        public string ReplyContent { get; set; }

        public int PostId { get; set; }       
        public string PostTitle { get; set; }
        public string PostContent { get; set; }

        public string ForumName { get; set; }
        public string ForumImageUrl { get; set; }
        public int ForumId { get; set; }
    }
}

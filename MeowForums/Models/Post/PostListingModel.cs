using MeowForums.Models.Reply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeowForums.Models.Post
{
    public class PostListingModel : PostBaseModel
    {
        public string DatePosted { get; set; }
        public ForumListingModel Forum { get; set; }
        public int RepliesCount { get; set; }
        public PostReplyModel LastReply { get; set; }
    }
}

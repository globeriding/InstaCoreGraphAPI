using System;
using System.Collections.Generic;

namespace InstaCoreGraphAPI.Graph.Entity
{
    public class SimpleMedia
    {
        public string id { get; set; }
        public int MediaDataId { get; set; }
        public string media_url { get; set; }
        public int comments_count { get; set; }
        public int like_count { get; set; }
        public int impression_count { get; set; }
        public string permalink { get; set; }
        public List<Comment> Comments { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime DateCreated { get; set; }

        public SimpleMedia()
        {
            this.Comments = new List<Comment>();
        }
    }
}

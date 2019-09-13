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
        /// <summary>
        ///  Total number of times the media object has been seen
        /// </summary>
        public int impression_count { get; set; }
        /// <summary>
        /// Total number of likes and comments on the media object
        /// </summary>
        public int engagement_count { get; set; }
        /// <summary>
        /// Total number of unique accounts that have seen the media object
        /// </summary>
        public int reach_count { get; set; }
        public string permalink { get; set; }
        public List<Comment> Comments { get; set; }
        public DateTime timestamp { get; set; }

        public SimpleMedia()
        {
            this.Comments = new List<Comment>();
        }
    }
}

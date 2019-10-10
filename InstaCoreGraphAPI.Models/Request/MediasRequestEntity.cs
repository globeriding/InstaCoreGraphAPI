using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InstaCoreGraphAPI.Graph.Entity.Request
{
    public class MediasRequestEntity
    {
        [DefaultValue(25)]
        public int limit { get; set; }
        [DefaultValue(null)]
        public string cursorBefore { get; set; }
        [DefaultValue(null)]
        public string cursorAfter { get; set; }
    }
}

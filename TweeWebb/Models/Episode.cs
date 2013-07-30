using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TweeWebb.Models
{
    public class Episodes
    {
        public string Season { get; set; }
        public string Episode { get; set; }
        public string Title { get; set; }
        public string Aired { get; set; }
        public string Watched { get; set; }
        public string EpisodeId { get; set; }
    }
}
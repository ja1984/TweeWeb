using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TweeWebb.Models
{
    public class Show
    {

        public string Summary { get; set; }
        public string SeriesId { get; set; }
        public string Name { get; set; }
        public string Actors { get; set; }
        public string Airs { get; set; }
        public string Genre { get; set; }
        public string ImdbId { get; set; }
        public string Rating { get; set; }
        public string Status { get; set; }
        public string FirstAired { get; set; }
        public List<Episodes> Episodes{ get; set; }

        public string GetImage()
        {
            return string.Format("http://www.thetvdb.com/banners/graphical/{0}.jpg",SeriesId);
        }

    }
}
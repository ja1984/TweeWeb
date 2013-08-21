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
        public List<Episodes> Episodes { get; set; }
        public List<Season> Seasons { get; set; }

        public string GetImage()
        {
            return string.Format("/headers/r_{0}.jpg", SeriesId);
        }

    }

    public class Season
    {
        public int Watched { get; set; }
        public int Unwatched { get; set; }
        public int Number { get; set; }
        public decimal Percent { get; set; }

        public string getSeason()
        {
            return Number == 0 ? "Specials" : string.Format("Season {0}", Number);
        }

    }

}
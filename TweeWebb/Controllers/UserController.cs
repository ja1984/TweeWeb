using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TweeWebb.Models;

namespace TweeWebb.Controllers
{
    public class UserController : RavenDbController
    {
        //
        // GET: /User/

        [HttpGet]
        public ActionResult Index(string userName)
        {
            var backup = RavenSession.Query<Backup>().FirstOrDefault(x => x.Username == userName);

            if (backup.Shows.Any(x => x.Seasons != null && x.Seasons.Any()))
                return View(backup);

            foreach (var show in backup.Shows)
            {
                var seasons = show.Episodes.GroupBy(x => x.Season);
                show.Seasons = new List<Season>();
                foreach (var season in seasons)
                {
                    var newSeason = new Season()
                                        {
                                            Number = int.Parse(season.Key),
                                            Unwatched = season.Count(x => x.Watched == "0"),
                                            Watched = season.Count(x => x.Watched == "1"),
                                        };
                    newSeason.Percent = CalculateWatchedPercentage(newSeason);
                    show.Seasons.Add(newSeason);


                }
            }

            RavenSession.Store(backup);
            RavenSession.SaveChanges();


            return View(backup);
        }




        private static decimal CalculateWatchedPercentage(Season season)
        {
            var episodes = season.Unwatched + season.Watched;
            decimal watchedEpisodes = season.Watched;
            decimal result = watchedEpisodes / episodes;
            return Math.Round(result * 100);

        }


    }


}

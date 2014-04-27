using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using PersonalTVGuide.Filters;
using PersonalTVGuide.InformationProviders;
using PersonalTVGuide.Models;
using PersonalTVGuide.TVShowObjects;
using WebGrease.Css.Ast.Selectors;
using WebMatrix.WebData;

namespace PersonalTVGuide.Controllers.Dashboard
{
    [InitializeSimpleMembership]
    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/
        private readonly SerieContext _db = new SerieContext();
        private readonly EpisodeContext _dbE = new EpisodeContext();
        
        [Authorize]
        public ActionResult Index()
        {
            //datum van vandaag en morgen pakken
            var today = DateTime.Now.Date;
            var tomorrow = DateTime.Now.Date.AddDays(1);

            //variabelen maken, lijst om de episodes in te kunnen stoppen
            var overview = new ListEpisodeAndSerieName();

            // query om de series van vandaag en morgen op te halen
            var allEpisodes = _dbE.Episodes.Where(e => e.Airdate == today || e.Airdate == tomorrow).ToList();
            var list = allEpisodes.Select(ep => new EpisodeAndSerieName
            {
                EpisodeId = ep.EpisodeId,
                EpisodeName = ep.EpisodeName,
                EpisodeNr = ep.EpisodeNR,
                EpisodeSeasonNr = ep.Season,
                EpisodeAirdate = ep.Airdate,
                SerieId = ep.SerieId,
                SerieName = _db.Series.FirstOrDefault(s => s.SerieId == ep.SerieId).SerieName
            }).ToList();
            // alle resultaten in overview stoppen
            overview.LstEpisodeAndSerieName = list;

            //overview resultaten door geven aan view
            return View(overview);
        }

        public ActionResult SearchShow()
        {
            // Haal eerst de resultaten op van de zoekactie
            var tvrip = new TvRageInformationProvider();
            List<TVRageShow> showList;
            try
            {
                showList = tvrip.GetShows(Convert.ToString(Request.Form["searchResult"]));
            }
            catch
            {
                return View("Error");
            }
            //telt hoeveel objecten in showlist staat en stopt het in viewbag
            ViewBag.ShowCount = showList.Count;

            // stopt alle results in ddlItems
            var ddlItems = showList.Select(s => new SelectListItem
            {
                Text = s.Name, Value = Convert.ToString(s.ShowId)
            }).ToList();

            ViewBag.ddlShows = ddlItems;
            Index();
            return View("Index");
        }

        [HttpPost]
        public ActionResult GetShowDetailsForDb()
        {
            try
            {
                // Haal de details op
                var tvrip = new TvRageInformationProvider();
                var show = tvrip.GetFullDetails(Convert.ToInt32(Request.Form["ddlShows"]));

                /*  Zet alle details in string om het op pagina te printen.
                    Is meer debug info, daarom uitgeschakeld!
                    Met @ViewBag.ShowResult kun je tonen op pagina */

                //var resultString = "";
                //resultString += ShowDetailsToString(show);
                //ViewBag.ShowResult = resultString;

                WriteToDatabase(show);
           
                ViewBag.ShowErrorMsg = "Opslaan in database is gelukt!";
                ViewBag.serie = show;
                Index();
                return View("index");
            }
            catch (InvalidOperationException err)
            {
                ViewBag.ShowErrorMsg = err.Message;
                return View("Index");
            }
        }

        //Zet een aflevering op checked wanneer deze is bekeken of niet
        public ActionResult GetShowDetails(int serieId = 0, int episodeId = -1)
        {
            var userId = WebSecurity.CurrentUserId;

            if (episodeId != -1)
            {
                var exists = _dbE.CheckedEpisodes.FirstOrDefault(c => c.UserId == userId && c.EpisodeId == episodeId);

                using (var dbC = new EpisodeContext())
                {
                    if (exists == null)
                        dbC.CheckedEpisodes.Add(new CheckedEpisodes
                        {
                            UserId = userId,
                            EpisodeId = episodeId
                        });
                    else
                    {
                        dbC.CheckedEpisodes.Attach(exists);
                        dbC.CheckedEpisodes.Remove(exists);
                    }

                    dbC.SaveChanges();
                }
            }

            var episodes = _dbE.Episodes.Where(e => e.SerieId == serieId).ToList();
            var checkedEpisodes = episodes.Select(episode => _dbE.CheckedEpisodes.FirstOrDefault(c => c.UserId == userId && c.EpisodeId == episode.EpisodeId)).Where(found => found != null).ToList();

            var sie = new SerieInfoAndEpisodes
            {
                Serie = _db.Series.First(t => t.SerieId == serieId),
                Episodes = episodes,
                CheckedEpisodes = checkedEpisodes
            };

            return View(sie);
        }


        // Functie wordt niet gebruik, kan als debug functioneren
        public string ShowDetailsToString(TVRageShow s)
        {
            var resultstring = "";

            resultstring += "Airtime H: " + Convert.ToString(s.AirTimeHour) + "<br />";
            resultstring += "AirTime M: " + Convert.ToString(s.AirTimeMinute) + "<br />";
            resultstring += "Country: " + s.Country + "<br />";
            resultstring += "Img URL: " + s.ImageUrl + "<br />";
            resultstring += "Link: " + s.Link + "<br />";
            resultstring += "Name: " + s.Name + "<br />";
            resultstring += "Season count: " + s.Seasons.Count + "<br />";
            resultstring += "Show ID: " + Convert.ToString(s.ShowId) + "<br />";
            resultstring += "<br /><br />";

            foreach (var z in s.Seasons)
            {
                resultstring += "### SEASON: " + z.SeasonNumber + "###<br />";
                foreach (var y in z.Episodes)
                {
                    resultstring += "Season nr.: " + z.SeasonNumber + "<br />";
                    resultstring += "Episode nr.: " + y.EpisodeNumberThisSeason + "<br />";
                    resultstring += "Episode Title: " + y.Title + "<br />";
                    resultstring += "Airdate: " + y.AirDate + "<br />";
                    resultstring += "<br /><br />";
                }
            }

            // mss nog uitbreiden??

            return resultstring;
        }

        [HttpPost]
        public ActionResult Favorite()
        {
            // pakt huide serieId
            var showId = Convert.ToInt32(Request.Form["hiddenShowId"]);
            using (var db = new SerieContext())
            {
                //voert een zoekopdracht uit in de database om te kijken of user als serie als favoriet heeft.
                var uhsExists = db.UserHasSeries.FirstOrDefault(s => s.SerieId == showId && s.UserId == WebSecurity.CurrentUserId);
                if (uhsExists == null)
                {
                    // voegt in table UserHasSerie serieId en bij de userId
                    db.UserHasSeries.Add(new UserHasSerie
                    {
                        UserId = WebSecurity.CurrentUserId,
                        SerieId = showId
                    });
                    db.SaveChanges();

                    ViewBag.ShowErrorMsg = "Serie is als favoriet toegevoegd!";
                }
                else
                {
                    ViewBag.ShowErrorMsg = "U heeft deze serie al als favoriet!";
                }
            }
            Index();
            return View("Index");
        }

        // methode voor udateknop.
        [HttpPost]
        public ActionResult Update()
        {
            var tvrip = new TvRageInformationProvider();
            // pakt ID van current serie
            var showId = Convert.ToInt32(Request.Form["hiddenShowId"]);
            // pakt show details die bij de serie hoort
            var show = tvrip.GetFullDetails(showId);
            //door loopt alle gegevens of ze nog kloppen. als episode nog niet in db staat, wordt die toegevoegd. 
            //wanneer die episode al wel bestaat wordt er gekeken of er dingen zijn verandert en past dit aan. 
                foreach (var z in show.Seasons)
                    foreach (var y in z.Episodes)
                    {
                        var episodeExists = _dbE.Episodes.FirstOrDefault(e => e.EpisodeNR == y.EpisodeNumberThisSeason && e.SerieId == show.ShowId && e.Season == z.SeasonNumber);

                        // voegt nieuwe episode toe als die nog niet in DB staat.
                        if (episodeExists == null)
                        {

                            _dbE.Episodes.Add(new Episode
                            {
                                SerieId = show.ShowId,
                                EpisodeNR = y.EpisodeNumberThisSeason,
                                EpisodeName = y.Title,
                                Airdate = y.AirDate,
                                Season = y.SeasonNumber,
                            });
                            _dbE.SaveChanges();
                        }

                        else
                        {
                            // overschrijft bestaande gegevens met nieuwe(wss altijd het zelfde)
                            episodeExists.SerieId = show.ShowId;
                            episodeExists.EpisodeNR = y.EpisodeNumberThisSeason;
                            episodeExists.EpisodeName = y.Title;
                            episodeExists.Airdate = y.AirDate;
                            episodeExists.Season = y.SeasonNumber;
                            _dbE.Entry(episodeExists).State = EntityState.Modified;
                            _dbE.SaveChanges();
                        }


                    }
            // message wanneer is geupdate
            ViewBag.ShowErrorMsg = "Serie is Geupdate";
            //roept methode aan om details van serie weer te laten zien
            GetShowDetails(showId);
            return View("GetShowDetails");
        }

        // Schrijf show + afleveringen weg naar database
        [HttpPost]
        public void WriteToDatabase(TVRageShow show)
        {
            if (ModelState.IsValid)
            {
                // Insert a new serie into the database
                using (var db = new SerieContext())
                using (var dbE = new EpisodeContext())
                {
                    Serie serieExists = db.Series.FirstOrDefault(s => s.SerieId == show.ShowId);
                    //Check if serie already exists
                    if (serieExists == null)
                    {
                        // Insert name into the serie table
                        db.Series.Add(new Serie
                        {
                            SerieId = show.ShowId,
                            SerieName = show.Name,
                            SerieSeasonCount = show.Seasons.Count().ToString(),
                            Runtime = show.Runtime,
                            IMG_url = show.ImageUrl,
                            Year = Convert.ToInt32(show.Started.Year),
                            status = show.Status
                        });

                        // save serie
                        db.SaveChanges();

                        foreach (var z in show.Seasons)
                            foreach (var y in z.Episodes)
                                dbE.Episodes.Add(new Episode
                                {
                                    SerieId = show.ShowId,
                                    EpisodeNR = y.EpisodeNumberThisSeason,
                                    EpisodeName = y.Title,
                                    Airdate = y.AirDate,
                                    Season = y.SeasonNumber,
                                });
                        dbE.SaveChanges();
                    }
                    else
                    {
                        foreach (var z in show.Seasons)
                            foreach (var y in z.Episodes)
                            {
                                var episodeExists = dbE.Episodes.FirstOrDefault(e => e.EpisodeNR == y.EpisodeNumberThisSeason && e.SerieId == show.ShowId && e.Season == z.SeasonNumber);
                                // voegt nieuwe episode toe als die nog niet in DB staat.
                                if (episodeExists == null)
                                {

                                    dbE.Episodes.Add(new Episode
                                    {
                                        SerieId = show.ShowId,
                                        EpisodeNR = y.EpisodeNumberThisSeason,
                                        EpisodeName = y.Title,
                                        Airdate = y.AirDate,
                                        Season = y.SeasonNumber,
                                    });
                                    dbE.SaveChanges();
                                }
                                else
                                {
                                    // overschrijft bestaande gegevens met nieuwe
                                    episodeExists.SerieId = show.ShowId;
                                    episodeExists.EpisodeNR = y.EpisodeNumberThisSeason;
                                    episodeExists.EpisodeName = y.Title;
                                    episodeExists.Airdate = y.AirDate;
                                    episodeExists.Season = y.SeasonNumber;
                                    dbE.Entry(episodeExists).State = EntityState.Modified;
                                    dbE.SaveChanges();
                                }


                            }
                       
                       
                    }
                }
            }
        }

    }
}

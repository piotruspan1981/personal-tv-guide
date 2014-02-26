using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PersonalTVGuide.Models;
using PersonalTVGuide.InformationProviders;
using PersonalTVGuide.TVShowObjects;
using WebMatrix.WebData;
using PersonalTVGuide.Filters;
using System.Data.Entity;

namespace PersonalTVGuide.Controllers
{
    [InitializeSimpleMembership]
    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/
        private SerieContext db = new SerieContext();
        private EpisodeContext dbE = new EpisodeContext();
        
        [Authorize]
        public ActionResult Index()
        {
            //datum van vandaag en morgen pakken
            DateTime today = DateTime.Now.Date;
            DateTime tomorrow = DateTime.Now.Date.AddDays(1);

            //variabelen maken, lijst om de episodes in te kunnen stoppen
            var overview = new ListSerieInfoAndEpisode();
            var list = new List<ObjSerieInfoAndEpisode>();

            // query om de series van vandaag en morgen optehalen
            var allEpisodes = dbE.Episodes.Where(e => e.Airdate == today || e.Airdate == tomorrow).ToList<Episode>();
            foreach (var ep in allEpisodes)
            {
                // toevoegen van gevonden resultaten
                var se = new ObjSerieInfoAndEpisode();
                se.EpisodeName = ep.EpisodeName;
                se.EpisodeNr = ep.EpisodeNR;
                se.EpisodeSeasonNr = ep.Season;
                se.EpisodeAirdate = ep.Airdate;
                se.SerieName = db.Series.FirstOrDefault(s => s.SerieId == ep.SerieId).SerieName;

                list.Add(se);
            }
            // alle resultaten in overview stoppen
            overview.LstSerieInfoAndEpisode = list;

            //var OverviewToday = new SerieInfoAndEpisodes();
            //OverviewToday.Episodes = dbE.Episodes.Where(e => e.Airdate == today || e.Airdate == tomorrow).ToList<Episode>();

            //overview resultaten door geven aan view
            return View(overview);
        }

        public ActionResult SearchShow()
        {
            // Haal eerst de resultaten op van de zoekactie
            var tvrip = new TvRageInformationProvider();
            var showList = new List<TVRageShow>();
            try
            {
                showList = tvrip.GetShows(Convert.ToString(Request.Form["searchResult"]));
            }
            catch
            {
                return View("Error");
            }

            ViewBag.ShowCount = showList.Count;

            var ddlItems = new List<SelectListItem>();

            foreach (var s in showList)
            {
                ddlItems.Add(new SelectListItem
                {
                    Text = s.Name,
                    Value = Convert.ToString(s.ShowId)
                });
            }

            ViewBag.ddlShows = ddlItems;
            Index();
            return View("Index");
        }

        [HttpPost]
        public ActionResult GetShowDetailsForDB()
        {
            try
            {
                // Haal de details op
                var tvrip = new TvRageInformationProvider();
                var show = new TVRageShow();

                show = tvrip.GetFullDetails(Convert.ToInt32(Request.Form["ddlShows"]));

                /*  Zet alle details in string om het op pagina te printen.
                    Is meer debug info, daarom uitgeschakeld!
                    Met @ViewBag.ShowResult kun je tonen op pagina */

                //var resultString = "";
                //resultString += ShowDetailsToString(show);
                //ViewBag.ShowResult = resultString;

                WriteToDatabase(show);
                //Favorite(show);

                /* OPHALEN UIT DATABASE TEST */
                //int serieid = Convert.ToInt32(Request.Form["ddlShows"]);
                //Serie serie = db.Series.First(t => t.SerieId == serieid);
                //ViewBag.serie = serie;
                
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

        //pakt de serie details van de gekozen serie.
        public ActionResult GetShowDetails(int id = 0)
        {
            //int serieid = id;
            //Serie serie = db.Series.First(t => t.SerieId == serieid);
            //Episode episode = dbE.Episodes.First(t => t.SerieId == serieid);
            
            var sie = new SerieInfoAndEpisodes();
            sie.Serie = db.Series.First(t => t.SerieId == id);
            sie.Episodes = dbE.Episodes.Where(e => e.SerieId == id).ToList<Episode>();

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
                    resultstring += "Episode nr.: " + y.EpisodeNumber + "<br />";
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
            using (SerieContext db = new SerieContext())
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
            var show = new TVRageShow();
            // pakt ID van current serie
            var showId = Convert.ToInt32(Request.Form["hiddenShowId"]);
            // pakt show details die bij de serie hoort
            show = tvrip.GetFullDetails(showId);
            //door loopt alle gegevens of ze nog kloppen. als episode nog niet in db staat, wordt die toegevoegd. 
            //wanneer die episode al wel bestaat wordt er gekeken of er dingen zijn verandert en past dit aan. 
                foreach (var z in show.Seasons)
                    foreach (var y in z.Episodes)
                    {
                        Episode episodeExists = null;
                        episodeExists = dbE.Episodes.FirstOrDefault(e => e.EpisodeName == y.Title);
                        // voegt nieuwe episode toe als die nog niet in DB staat.
                        if (episodeExists == null)
                        {

                            dbE.Episodes.Add(new Episode
                            {
                                SerieId = show.ShowId,
                                EpisodeNR = y.EpisodeNumber,
                                EpisodeName = y.Title,
                                Airdate = y.AirDate,
                                Season = y.SeasonNumber,
                            });
                            dbE.SaveChanges();
                        }
                        else
                        {
                            // overschrijft bestaande gegevens met nieuwe(wss altijd het zelfde)
                            episodeExists.SerieId = show.ShowId;
                            episodeExists.EpisodeNR = y.EpisodeNumber;
                            episodeExists.EpisodeName = y.Title;
                            episodeExists.Airdate = y.AirDate;
                            episodeExists.Season = y.SeasonNumber;
                            dbE.Entry(episodeExists).State = EntityState.Modified;
                            dbE.SaveChanges();
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
                using (SerieContext db = new SerieContext())
                using (EpisodeContext dbE = new EpisodeContext())
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
                                    EpisodeNR = y.EpisodeNumber,
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
                                Episode episodeExists = null;
                                episodeExists = dbE.Episodes.FirstOrDefault(e => e.EpisodeName == y.Title);
                                // voegt nieuwe episode toe als die nog niet in DB staat.
                                if (episodeExists == null)
                                {

                                    dbE.Episodes.Add(new Episode
                                    {
                                        SerieId = show.ShowId,
                                        EpisodeNR = y.EpisodeNumber,
                                        EpisodeName = y.Title,
                                        Airdate = y.AirDate,
                                        Season = y.SeasonNumber,
                                    });
                                    dbE.SaveChanges();
                                }
                                else
                                {
                                    // overschrijft bestaande gegevens met nieuwe(wss altijd het zelfde)
                                    episodeExists.SerieId = show.ShowId;
                                    episodeExists.EpisodeNR = y.EpisodeNumber;
                                    episodeExists.EpisodeName = y.Title;
                                    episodeExists.Airdate = y.AirDate;
                                    episodeExists.Season = y.SeasonNumber;
                                    dbE.Entry(episodeExists).State = EntityState.Modified;
                                    dbE.SaveChanges();
                                }


                            }
                       
                       

                           //TO DO!!
                           ViewBag.ShowErrorMsg = "Serie bestaat al in database!";
                       
                    }
                }
            }
        }

    }
}

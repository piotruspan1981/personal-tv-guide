﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PersonalTVGuide.InformationProviders;
using PersonalTVGuide.TVShowObjects;
using PersonalTVGuide.Models;

namespace PersonalTVGuide.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Testen()
        {
            // Haal eerst de resultaten op van de zoekactie
            var tvrip = new TvRageInformationProvider();
            var showList = new List<Show>();
            try
            {
                showList = tvrip.GetShows(Convert.ToString(Request.Form["searchResult"]));
            }
            catch
            {
                return View("Error");
            }
            
            ViewBag.ShowCount = "Aantal gevonden shows: " + showList.Count;

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

            return View();
        }

        [HttpPost]
        public ActionResult ShowAllDetails()
        {
            // Haal de details op
            var tvrip = new TvRageInformationProvider();
            var show = new Show();

            show = tvrip.GetFullDetails(Convert.ToInt32(Request.Form["ddlShows"]));
            var resultString = "";

            resultString += getShow(show);
            WriteToDatabase(show);

            ViewBag.ShowResult = resultString;

            return View();
        }

        public string getShow(Show s)
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

        public void WriteToDatabase(Show show)
        {
            if (ModelState.IsValid)
            {
                // Insert a new serie into the database
                using (SerieContext db = new SerieContext())
                {
                    Serie serieExists = db.Serie.FirstOrDefault(s => s.SerieName == show.Name);
                    // Check if serie already exists
                    if (serieExists == null)
                    {
                        // Insert name into the serie table
                        db.Serie.Add(new Serie { SerieName = show.Name, SerieSeasonCount = show.Seasons.Count().ToString()});
                        db.SaveChanges();
                    }
                    else
                    {
                        //TO DO!!
                        //ViewBag.ShowErrorMsg = "Serie bestaat al in database!";
                    }
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PersonalTVGuide.InformationProviders;
using PersonalTVGuide.TVShowObjects;

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
            var resultString = "";
            var bla = new TvRageInformationProvider();
            var showList = new List<Show>();
            showList = bla.GetShows("Lost");
            
            ViewBag.ShowCount = "Aantal gevonden shows: " + showList.Count;

            foreach (var s in showList)
            {
                resultString += getShow(s);
            }

            ViewBag.Results = resultString;

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

            return resultstring;
        }

    }
}

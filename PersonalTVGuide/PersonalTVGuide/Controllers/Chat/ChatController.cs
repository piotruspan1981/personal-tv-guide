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
    public class ChatController : Controller
    {
        private ChatContext db = new ChatContext();

        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Shoutbox(Shoutbox shout)
        {
            
            if (ModelState.IsValid)
            {
                db.Shoutbox.Add(new Shoutbox
                {
                    UID = WebSecurity.CurrentUserId,
                    DateAndTime = DateTime.Now,
                    Text = shout.Text

                });

               
              db.SaveChanges();
              return View("index");
                
            }
            return View("index");
        }

    }
}

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
        private UsersContext dbU = new UsersContext();
        public ActionResult Index()
        {
            var shoutOverview = new ListShoutboxDisplay();
            var list = new List<ObjShoutboxDisplay>();
            
            var queryshout = db.Shoutbox.Select(s => new ObjShoutboxDisplay { DateAndTime = s.DateAndTime, Text = s.Text, UID = s.UID })
                .OrderByDescending(o => o.DateAndTime)
                .Take(20)
                .ToList();
           // var test2 = dbU.UserProfiles.Where(u => u.UserId == test ).Select(s => new ShoutboxDisplay { UserName = s.UserName}
            foreach (var shout in queryshout)
            {
                var t = new ObjShoutboxDisplay();
                t.Text = shout.Text;
                t.DateAndTime = shout.DateAndTime;
                t.UserName = dbU.UserProfiles.FirstOrDefault(u => u.UserId == shout.UID).UserName;

                list.Add(t);
            }
            shoutOverview.LstShoutboxDisplay = list;
          
            return View(shoutOverview);
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
              ModelState.Clear();
              Index();
              return View("index");
                
            }
            Index();
            return View("index");
        }

    }
}

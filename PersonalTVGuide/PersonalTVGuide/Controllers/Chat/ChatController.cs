﻿using System;
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
            // list waar de shouts inkomen
            var shoutOverview = new ListShoutboxDisplay();
            // list waar de objects inkomen van de query
            var list = new List<ObjShoutboxDisplay>();
            //query die juistse gegevens selecteerd uit database
            var queryshout = db.Shoutbox.Select(s => new ObjShoutboxDisplay { DateAndTime = s.DateAndTime, Text = s.Text, UID = s.UID })
                .OrderByDescending(o => o.DateAndTime)
                .Take(20)
                .ToList();
           
            //loopt door de query reultaat en voegt de resultaten toe aan objecten zodat ze getoond kunnen worden
            foreach (var shout in queryshout)
            {
                var t = new ObjShoutboxDisplay();
                t.Text = shout.Text;
                t.DateAndTime = shout.DateAndTime;
                t.UserName = dbU.UserProfiles.FirstOrDefault(u => u.UserId == shout.UID).UserName;

                list.Add(t);
            }
            // stopt resultaat van list in de overview voor weergave
            shoutOverview.LstShoutboxDisplay = list;
          
            return View(shoutOverview);
        }
        
        // toevoegen van shouts aan db
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
              return RedirectToAction("Index");
                
            }
            
            Index();
            return View("index");
        }

       // methode voor berichten laten zien
        public ActionResult PrivatemessageInput()
        {
            
            var privateMsg = new ListPrivateMsg();
            var list = new List<ObjPrivateMsg>();

            //query die de berichten ophaald voor ingelogde gebruiker, sorteert het zo dat nieuweste berichtne boven aan staan.
            var querypmsg = db.PrivateMsg.Where(u => u.ReceiverID == WebSecurity.CurrentUserId)
                .Select(p => new ObjPrivateMsg
            {
                MsgID = p.MsgID,
                DateAndTime = p.DateAndTime,
                Opened = p.Opened,
                Text = p.Text,
                SUID = p.SenderID
            })
                .OrderByDescending(o => o.DateAndTime)
                .ToList();

            //opslpitsen van query resultaten naar objecten. een voor een toevoegen aan lijst
            foreach (var msg in querypmsg)
            {
                var ObjPmsg = new ObjPrivateMsg();
                ObjPmsg.MsgID = msg.MsgID;
                ObjPmsg.Text = msg.Text;
                ObjPmsg.DateAndTime = msg.DateAndTime;
                ObjPmsg.Opened = msg.Opened;
                ObjPmsg.SenderName = dbU.UserProfiles.FirstOrDefault(u => u.UserId == msg.SUID).UserName;
               

                list.Add(ObjPmsg);
            }

            //verwerken van message uit privatemessage methode
            if (TempData["shortMessage"] != null)
            {
                ViewBag.ShowErrorMsg = TempData["shortMessage"].ToString();
            }

            
            privateMsg.LstPrivateMsg = list;
            return View(privateMsg);
        }


        // methode om bericht te verwijderen
        public ActionResult DeleteMsg(int id = 0)
        {
            PrivateMsg Dmsg = db.PrivateMsg.Find(id);
            if (Dmsg != null)
            {
                db.PrivateMsg.Remove(Dmsg);
                db.SaveChanges();
            }
            return RedirectToAction("PrivatemessageInput");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrivateMessage(PrivateMsg PMsg)
        {
            // ophalen van ingevoerde naam
            var naam = Request.Form["Naam"];
            try
            {
                //UserId pakken van de ingevoerde naam
                var ontvanger = dbU.UserProfiles.FirstOrDefault(u => u.UserName == naam).UserId;
                db.PrivateMsg.Add(new PrivateMsg
                {

                    SenderID = WebSecurity.CurrentUserId,
                    ReceiverID = ontvanger,
                    DateAndTime = DateTime.Now,
                    Text = PMsg.Text
                });
                db.SaveChanges();
                ModelState.Clear();
                //bericht in tempdata zetten zodat de andere methode hem kan gebruiken
                TempData["shortMessage"] = "Bericht is verzonden!";
               
            }
            catch
            {
                ModelState.Clear();
                TempData["shortMessage"] = "De ontvanger bestaat niet.";
                
            }

            
            return RedirectToAction("PrivatemessageInput");
        }
    }

}

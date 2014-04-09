using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using PersonalTVGuide.Filters;
using PersonalTVGuide.Models;
using CaptchaMvc.Attributes;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc.Interface;
using Postal;
using System.Data.Entity;

namespace PersonalTVGuide.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        private UsersContext db = new UsersContext();
        private SerieContext dbS = new SerieContext();
        private EpisodeContext dbE = new EpisodeContext();
        private GlobalContext global = new GlobalContext();
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                using (UsersContext db = new UsersContext())
                {
                    UserProfile userProfile = db.UserProfiles.SingleOrDefault(u => u.UserName == model.UserName);
                    userProfile.LastOnline = DateTime.Now;
                    db.Entry(userProfile).State = EntityState.Modified;
                    db.SaveChanges();
                }

                // Haal de datum en tijd op wanneer de notificaties het laatst zijn verzonden
                var notificationsSendDateTime = Convert.ToDateTime(global.GlobalSettings.SingleOrDefault(g => g.Name == "NotificationsSend").Value);
                if (notificationsSendDateTime.Date < DateTime.Now.Date)
                {
                    FillEpisodesForNotificationAndSendMail();

                    // Update de notificatie tijd, zodat de notification maar 1x per dag worden verstuurd naar een gebruiker
                    using (GlobalContext glb = new GlobalContext())
                    {
                        var notificationSend = glb.GlobalSettings.SingleOrDefault(g => g.Name == "NotificationsSend");
                        notificationSend.Value = DateTime.Now;
                        glb.SaveChanges();
                    }       
                }       

                //return RedirectToLocal(returnUrl);
                return RedirectToAction("Index", "Dashboard");
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        private void FillEpisodesForNotificationAndSendMail()
        {
            var today = DateTime.Now;

            var usersWithDailyNotifications = db.UserProfiles.Where(u => u.NotificationFreq == 0 || u.NotificationFreq == 2).ToList<UserProfile>();
            var allEpisodesDaily = dbE.Episodes.Where(e => e.Airdate == today.Date).ToList<Episode>();

            foreach (var user in usersWithDailyNotifications)
            {
                var userHasSeries = dbS.UserHasSeries.Where(uhs => uhs.UserId == user.UserId).ToList<UserHasSerie>();
                var listEpisodesAndSerieName = new List<EpisodeAndSerieName>();

                foreach (var uhs in userHasSeries)
                {
                    var listES = allEpisodesDaily.Where(e => e.SerieId == uhs.SerieId)
                        .Join(dbS.Series, e => e.SerieId, s => s.SerieId, (e, s) => new { e.EpisodeName, e.EpisodeNR, e.Season, e.Airdate, s.SerieName })
                        .Select(es => new EpisodeAndSerieName
                        {
                            EpisodeName = es.EpisodeName,
                            EpisodeNr = es.EpisodeNR,
                            EpisodeSeasonNr = es.Season,
                            EpisodeAirdate = es.Airdate,
                            SerieName = es.SerieName
                        }).ToList();

                    foreach (var es in listES)
                        listEpisodesAndSerieName.Add(es);
                }

                if (listEpisodesAndSerieName.Count != 0)
                {
                    // Verstuur email in HTML
                    dynamic email = new Email("NotificationEmailDaily");
                    email.To = user.Email;
                    email.UserName = user.UserName;
                    var lstES = new ListEpisodeAndSerieName();
                    lstES.LstEpisodeAndSerieName = listEpisodesAndSerieName;
                    email.episodes = lstES; // vieze hackzz!!
                    email.Send();
                }
            }

            if (today.DayOfWeek == DayOfWeek.Monday)
            {
                var usersWithWeeklyNotifications = db.UserProfiles.Where(u => u.NotificationFreq == 1 || u.NotificationFreq == 2).ToList<UserProfile>();

                DateTime week = DateTime.Now.Date.AddDays(7);
                var allEpisodesWeekly = dbE.Episodes.Where(e => e.Airdate >= today.Date && e.Airdate <= week).ToList<Episode>();

                foreach (var user in usersWithWeeklyNotifications)
                {
                    var userHasSeries = dbS.UserHasSeries.Where(uhs => uhs.UserId == user.UserId).ToList<UserHasSerie>();
                    var listEpisodesAndSerieName = new List<EpisodeAndSerieName>();

                    foreach (var uhs in userHasSeries)
                    {
                        var listES = allEpisodesWeekly.Where(e => e.SerieId == uhs.SerieId)
                            .Join(dbS.Series, e => e.SerieId, s => s.SerieId, (e, s) => new { e.EpisodeName, e.EpisodeNR, e.Season, e.Airdate, s.SerieName })
                            .Select(se => new EpisodeAndSerieName
                            {
                                EpisodeName = se.EpisodeName,
                                EpisodeNr = se.EpisodeNR,
                                EpisodeSeasonNr = se.Season,
                                EpisodeAirdate = se.Airdate,
                                SerieName = se.SerieName
                            }).ToList();

                        foreach (var es in listES)
                            listEpisodesAndSerieName.Add(es);
                    }

                    if (listEpisodesAndSerieName.Count != 0)
                    {
                        // Verstuur email in HTML
                        dynamic email = new Email("NotificationEmailWeekly");
                        email.To = user.Email;
                        email.UserName = user.UserName;
                        var lstES = new ListEpisodeAndSerieName();
                        lstES.LstEpisodeAndSerieName = listEpisodesAndSerieName;
                        email.episodes = lstES;
                        email.Send();
                    }
                }
            }
        }

        // aanmaken van methode voor favoriete series overzicht.
        public List<UserSerieFavorites> FavSeries()
        {
            
            var FavOverview = new List<UserSerieFavorites>();
         
            /*
             * User has series -> gegevens van deze serie
             */
            //query die uit 2 tables informatie haalt met behulp van join, 
            //van table serie seriename en serieid, en van table userhasserie het id
            FavOverview = dbS.UserHasSeries.Where(s => s.UserId == WebSecurity.CurrentUserId)
                .Join(dbS.Series, u => u.SerieId, s => s.SerieId, (u, s) => new { s.SerieName, s.SerieId, u.Id } )
                .Select(s => new UserSerieFavorites { SerieName = s.SerieName, SerieId = s.SerieId, UhasSID = s.Id}).ToList();

         

            return FavOverview;
        }
       

        // methode om favoriete serie te deleten
        public ActionResult Delete(int id = 0)
        {
            // zoek in userHasSeries table naar match van de meegegeven ID
            UserHasSerie fav = dbS.UserHasSeries.Find(id);
            if (fav == null)
            {
                return HttpNotFound();
            }
            else
            {   
                //verwijderd gekozen data uit DB
                dbS.UserHasSeries.Remove(fav);
                dbS.SaveChanges();
            }
            Profile();
            return View("Profile");
        }

       


        public ActionResult Profile()
        {
            // pakt UserID van ingelogde user
            object Userid = Membership.GetUser().ProviderUserKey;
            // zoekt informatie uit DB die bij de UserID hoort
            UserProfile profile = db.UserProfiles.Find(Userid);
                
            if (profile == null)
            {
                return HttpNotFound();
            }
            // geeft de waardes die in profile staan door aan de view
            ViewBag.Favseries = FavSeries();
            return View(profile);
        }

        public ActionResult Edit()
        {
            // pakt UserID van ingelogde user
            object Userid = Membership.GetUser().ProviderUserKey;
            // zoekt informatie uit DB die bij de UserID hoort
            UserProfile profile = db.UserProfiles.Find(Userid);

            if (profile == null)
            {
                return HttpNotFound();
            }
            // geeft de waardes die in profile staan door aan de view
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserProfile profile)
        {
            if (ModelState.IsValid)
            {
                
                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Profile");
            }
            return View(profile);
        }
        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost, CaptchaVerify("Captcha is not valid")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    //// Nieuwe uitbreidingen op het Registeren pagina moeten  binnen new { model.Email, model.blaat, model.asd }
                    //// Anders klopt de 3e parameter van CreateUserAndAccount niet. Dat is een dictionary met objects property values.
                    
                    string confirmationToken =
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { Email = model.Email }, true);
                    dynamic email = new Email("RegEmail");
                    email.To = model.Email;
                    email.UserName = model.UserName;
                    email.ConfirmationToken = confirmationToken;
                    email.Send();

                    return RedirectToAction("RegisterStepTwo", "Account");

                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult RegisterStepTwo()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult RegisterConfirmation(string Id)
        {
            if (WebSecurity.ConfirmAccount(Id))
            {
                return RedirectToAction("ConfirmationSuccess");
            }
            return RedirectToAction("ConfirmationFailure");
        }

        [AllowAnonymous]
        public ActionResult ConfirmationSuccess()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ConfirmationFailure()
        {
            return View();
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

       

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}

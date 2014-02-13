using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace PersonalTVGuide.TVShowObjects
{
    public class Show
    {
        public List<Season> Seasons { get; set; }

        private string country;
        private System.DateTime? ended;
        //private ImageSource imageSource;
        private string link;
        private string name;
        private int showId;
        private System.DateTime? started;
        private string status;
        private string imageUrl;
        private bool isLoading;
        private Season selectedSeason;
        private int airTimeHour;
        private int airTimeMinute;
        private int timezone;
        private int runtime;

        public int Runtime
        {
            get { return runtime; }
            set
            {
                runtime = value;
            }
        }

        public int Timezone
        {
            get { return timezone; }
            set
            {
                timezone = value;
            }
        }

        public int AirTimeMinute
        {
            get { return airTimeMinute; }
            set
            {
                airTimeMinute = value;
            }
        }
 
        public int AirTimeHour
        {
            get { return airTimeHour; }
            set
            {
                airTimeHour = value;
            }
        }


        public Show()
        {
            Seasons = new List<Season>();
        }

        public Season SelectedSeason
        {
            get { return selectedSeason; }
            set
            {
                selectedSeason = value;
            }
        }
        
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
            }
        }
        
        public string ImageUrl
        {
            get { return imageUrl; }
            set
            {
                imageUrl = value;
            }
        }

        public System.DateTime? Started
        {
            get { return started; }
            set
            {
                started = value;
            }
        }

        public System.DateTime? Ended
        {
            get { return ended; }
            set
            {
                ended = value;
            }
        }

        //public ImageSource ImageSource
        //{
        //    get { return imageSource; }
        //    set
        //    {
        //        imageSource = value;
        //        NotifyOfPropertyChange(() => ImageSource);
        //    }
        //}

        public string Status
        {
            get { return status; }
            set
            {
                status = value;
            }
        }

        public string Country
        {
            get { return country; }
            set
            {
                country = value;
            }
        }

        public string Link
        {
            get { return link; }
            set
            {
                link = value;
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }

        public int ShowId
        {
            get { return showId; }
            set
            {
                showId = value;
            }
        }
    }
}
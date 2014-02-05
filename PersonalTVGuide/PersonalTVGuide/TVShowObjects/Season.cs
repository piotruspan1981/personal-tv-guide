using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace PersonalTVGuide.TVShowObjects
{
    public class Season
    {
        public List<Episode> Episodes { get; set; }
        private int seasonNumber;

        private Episode selectedEpisode;
         
        public Episode SelectedEpisode
        {
            get { return selectedEpisode; }
            set
            {
                selectedEpisode = value;
            }
        }

        public int SeasonNumber
        {
            get { return seasonNumber; }
            set
            {
                seasonNumber = value;
            }
        }
        
        public Season()
        {
            Episodes = new List<Episode>();
        }

    }
}
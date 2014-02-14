using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace PersonalTVGuide.TVShowObjects
{
    public class TVRageSeason
    {
        public List<TVRageEpisode> Episodes { get; set; }
        private int seasonNumber;

        private TVRageEpisode selectedEpisode;
         
        public TVRageEpisode SelectedEpisode
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
        
        public TVRageSeason()
        {
            Episodes = new List<TVRageEpisode>();
        }

    }
}
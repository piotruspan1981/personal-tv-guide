using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace PersonalTVGuide.TVShowObjects
{
    public class Episode
    {
        private DateTime? airDate;
        private int episodeNumber;
        private int episodeNumberThisSeason;
        private bool hasBeenViewed;
        //private ImageSource imageSource;
        private string imageUrl;
        private bool isLoading;
        private string link;
        private int seasonNumber;
        private string showName;
        private string title;

        public int EpisodeNumberThisSeason
        {
            get { return episodeNumberThisSeason; }
            set
            {
                episodeNumberThisSeason = value;
            }
        }

        public bool HasBeenViewed
        {
            get { return hasBeenViewed; }
            set
            {
                hasBeenViewed = value;
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

        public string ShowName
        {
            get { return showName; }
            set
            {
                showName = value;
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

        //public ImageSource ImageSource
        //{
        //    get { return imageSource; }
        //    set
        //    {
        //        imageSource = value;
        //        NotifyOfPropertyChange(() => ImageSource);
        //    }
        //}

        public string ImageUrl
        {
            get { return imageUrl; }
            set
            {
                imageUrl = value;
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

        public DateTime? AirDate
        {
            get { return airDate; }
            set
            {
                airDate = value;
            }
        }

        public int EpisodeNumber
        {
            get { return episodeNumber; }
            set
            {
                episodeNumber = value;
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
            }
        }
    }
}
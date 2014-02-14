using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace PersonalTVGuide.Models
{
    public class EpisodeContext : DbContext
    {
        public EpisodeContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Episode> Episodes { get; set; }
    }

    [Table("Episodes")]
    public class Episode
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int EpisodeId { get; set; }
        public int SerieId { get; set; }
        public string EpisodeName { get; set; }
        public int Season { get; set; }
        public int EpisodeNR{ get; set; }
        public DateTime Airdate { get; set; }
        //public string IMG_url { get; set; }


    }

}
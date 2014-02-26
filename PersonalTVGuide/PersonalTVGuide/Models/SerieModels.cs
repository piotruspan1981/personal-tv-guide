using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace PersonalTVGuide.Models
{
    public class SerieContext : DbContext
    {
        public SerieContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Serie> Series { get; set; }
        public DbSet<UserHasSerie> UserHasSeries { get; set; }
    }

    [Table("Serie")]
    public class Serie
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SerieId { get; set; }
        [StringLength(200), Required]
        public string SerieName { get; set; }
        [StringLength(20), Required]
        public string SerieSeasonCount { get; set; }
        public int Runtime { get; set; }
        [StringLength(500)]
        public string IMG_url { get; set; }
        public int Year { get; set; }
        [StringLength(20), Required]
        public string status { get; set; }
    }

   [Table("UserHasSerie")]
    public class UserHasSerie
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SerieId { get; set; }
    }

   public class UserSerieFavorites
   {
       public int SerieId { get; set; }
       public string SerieName { get; set; }
   }

   public class SerieInfoAndEpisodes
   {
       public Serie Serie { get; set; }
       public List<Episode> Episodes { get; set; }
   }

   public class ListSerieInfoAndEpisode
   {
       public List<ObjSerieInfoAndEpisode> LstSerieInfoAndEpisode { get; set; }
   }

   public class ObjSerieInfoAndEpisode
   {
       public string SerieName { get; set; }
       public int EpisodeSeasonNr { get; set; }
       public int EpisodeNr { get; set; }
       public string EpisodeName { get; set; }
       public DateTime? EpisodeAirdate { get; set; }
   }

}
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
    }

        [Table("Serie")]
        public class Serie
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }
            public int SerieId { get; set; }
            public string SerieName { get; set; }
            public string SerieSeasonCount { get; set; }
            public int Runtime { get; set; }
            public string IMG_url { get; set; }
            public int Year { get; set; }
        }

}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace PersonalTVGuide.Models
{
    public class GlobalContext : DbContext
    {
        public GlobalContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<GlobalSettings> GlobalSettings { get; set; }
    }

    [Table("GlobalDBSettings")]
    public class GlobalSettings
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Value { get; set; }
    }
}
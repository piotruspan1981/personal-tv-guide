using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace PersonalTVGuide.Models
{
    public class ShoutboxModels
    {
        public int ID { get; set; }
        public int UID { get; set; }
        public DateTime DateAndTime { get; set; }
        public string Text { get; set; }
    }

    public class ShoutboxDBContext : DbContext
    {
        public DbSet<ShoutboxModels> Shoutbox { get; set; }
    }
}
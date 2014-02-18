using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PersonalTVGuide.Models
{
    public class ChatContext : DbContext
    {
        public ChatContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Shoutbox> Shoutbox{ get; set; }
        public DbSet<PrivateMsg> PrivateMsg { get; set; }
    }
    public class Shoutbox
    {
        public int ID { get; set; }
        public int UID { get; set; }
        public DateTime DateAndTime { get; set; }
        public string Text { get; set; }
    }

    public class PrivateMsg
    {
        public int MsgID { get; set; }
        public int SenderID { get; set; }
        public int ReceiverID {get; set;}
        public DateTime DateAndTime { get; set; }
        public string Text { get; set; }
        public bool Opened{ get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

    [Table("Shoutbox")]
    public class Shoutbox
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int UID { get; set; }
        public DateTime DateAndTime { get; set; }
        [Required]
        public string Text { get; set; }
    }

    [Table("PrivateMsg")]
    public class PrivateMsg
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MsgID { get; set; }
        public int SenderID { get; set; }
        public int ReceiverID {get; set;}
        public DateTime DateAndTime { get; set; }
        [Required]
        public string Text { get; set; }
        public bool Opened{ get; set; }
    }
}
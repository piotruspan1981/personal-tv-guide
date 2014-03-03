namespace PersonalTVGuide.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chat : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PrivateMsg",
                c => new
                    {
                        MsgID = c.Int(nullable: false, identity: true),
                        SenderID = c.Int(nullable: false),
                        ReceiverID = c.Int(nullable: false),
                        DateAndTime = c.DateTime(nullable: false),
                        Text = c.String(nullable: false),
                        Opened = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MsgID);
            
            CreateTable(
                "dbo.Shoutbox",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UID = c.Int(nullable: false),
                        DateAndTime = c.DateTime(nullable: false),
                        Text = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            
        }
        
        public override void Down()
        {
          
        }
    }
}

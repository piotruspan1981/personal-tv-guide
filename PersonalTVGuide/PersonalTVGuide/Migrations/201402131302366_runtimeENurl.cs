namespace PersonalTVGuide.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class runtimeENurl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Serie", "Runtime", c => c.Int(nullable: false));
            AddColumn("dbo.Serie", "IMG_url", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Serie", "IMG_url");
            DropColumn("dbo.Serie", "Runtime");
        }
    }
}

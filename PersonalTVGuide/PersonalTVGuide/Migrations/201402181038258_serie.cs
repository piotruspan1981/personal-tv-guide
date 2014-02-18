namespace PersonalTVGuide.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class serie : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.UserHasSerie", newName: "UserHasSerie");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.UserHasSerie", newName: "UserHasSeries");
        }
    }
}

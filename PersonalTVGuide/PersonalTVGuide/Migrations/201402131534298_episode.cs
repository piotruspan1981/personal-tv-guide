namespace PersonalTVGuide.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class episode : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Episodes",
                c => new
                    {
                        EpisodeId = c.Int(nullable: false, identity: true),
                        SerieId = c.Int(nullable: false),
                        EpisodeName = c.String(),
                        EpisodeNR = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EpisodeId);
            
            //DropTable("dbo.Serie");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Serie",
                c => new
                    {
                        SerieId = c.Int(nullable: false, identity: true),
                        SerieName = c.String(),
                        SerieSeasonCount = c.String(),
                        Runtime = c.Int(nullable: false),
                        IMG_url = c.String(),
                    })
                .PrimaryKey(t => t.SerieId);
            
            DropTable("dbo.Episodes");
        }
    }
}

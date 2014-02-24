namespace PersonalTVGuide.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class airdatenull : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CheckedEpisodes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        EpisodeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Episodes",
                c => new
                    {
                        EpisodeId = c.Int(nullable: false, identity: true),
                        SerieId = c.Int(nullable: false),
                        EpisodeName = c.String(nullable: false, maxLength: 200),
                        Season = c.Int(nullable: false),
                        EpisodeNR = c.Int(nullable: false),
                        Airdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.EpisodeId);
            
           
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserHasSerie",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        SerieId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Serie",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SerieId = c.Int(nullable: false),
                        SerieName = c.String(nullable: false, maxLength: 200),
                        SerieSeasonCount = c.String(nullable: false, maxLength: 20),
                        Runtime = c.Int(nullable: false),
                        IMG_url = c.String(maxLength: 500),
                        Year = c.Int(nullable: false),
                        status = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.Episodes");
            DropTable("dbo.CheckedEpisodes");
        }
    }
}

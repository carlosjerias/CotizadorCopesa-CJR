namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambioenelAnchoBandaPapel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Papel", "AnchoBanda", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Papel", "AnchoBanda", c => c.Int(nullable: false));
        }
    }
}

namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambiodelomoxmicron : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Papel", "Micron", c => c.Double(nullable: false));
            DropColumn("dbo.Papel", "Lomo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Papel", "Lomo", c => c.Double(nullable: false));
            DropColumn("dbo.Papel", "Micron");
        }
    }
}

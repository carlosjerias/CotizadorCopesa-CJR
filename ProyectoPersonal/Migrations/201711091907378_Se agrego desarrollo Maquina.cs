namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeagregodesarrolloMaquina : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Maquina", "Desarrollo", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Maquina", "Desarrollo");
        }
    }
}

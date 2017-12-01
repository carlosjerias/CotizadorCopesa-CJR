namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Seagregoapapelprecioxkilos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Papel", "PrecioKilos", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Papel", "PrecioKilos");
        }
    }
}

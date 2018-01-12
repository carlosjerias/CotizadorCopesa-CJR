namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seadicionoelcampototaldespachoaPPTO : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Presupuesto", "TarifaFijaDespacho", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Presupuesto", "TarifaFijaDespacho");
        }
    }
}

namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tarifaManufactura : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Presupuesto", "ManufacturaFijo", c => c.Double(nullable: false));
            AddColumn("dbo.Presupuesto", "ManufacturaVari", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Presupuesto", "ManufacturaVari");
            DropColumn("dbo.Presupuesto", "ManufacturaFijo");
        }
    }
}

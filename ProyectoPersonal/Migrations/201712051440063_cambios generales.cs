namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambiosgenerales : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Interior", "CostoPapelinteriorFijo", c => c.Double(nullable: false));
            AddColumn("dbo.Interior", "CostoPapelInteriorVari", c => c.Double(nullable: false));
            AddColumn("dbo.Tapa", "CostoPapelTapaFijo", c => c.Double(nullable: false));
            AddColumn("dbo.Tapa", "CostoPapelTapaVari", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tapa", "CostoPapelTapaVari");
            DropColumn("dbo.Tapa", "CostoPapelTapaFijo");
            DropColumn("dbo.Interior", "CostoPapelInteriorVari");
            DropColumn("dbo.Interior", "CostoPapelinteriorFijo");
        }
    }
}

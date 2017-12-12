namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambiosenCatalogoparaautomatizarelsistema : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TipoCatalogo", "PapelInterior", c => c.String());
            AddColumn("dbo.TipoCatalogo", "PapelTapa", c => c.String());
            AddColumn("dbo.Presupuesto", "CantidadenBolsa", c => c.Int(nullable: false));
            AddColumn("dbo.Presupuesto", "LibrosxCajas", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Presupuesto", "LibrosxCajas");
            DropColumn("dbo.Presupuesto", "CantidadenBolsa");
            DropColumn("dbo.TipoCatalogo", "PapelTapa");
            DropColumn("dbo.TipoCatalogo", "PapelInterior");
        }
    }
}

namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tablapresupuestoagregueterminaciones : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "BarnizUVBrillante", c => c.Int());
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "BarnizUVBrillanteSelectivo", c => c.Int());
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "BarnizGlitterSelectivo", c => c.Int());
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "PolitermolaminadoBrillante", c => c.Int());
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "PolitermolaminadoOpaco", c => c.Int());
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "Plisado", c => c.Int());
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "Troquel", c => c.Int());
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "Corte", c => c.Int());
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "DoblezDiptico", c => c.Int());
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "Adhesivo", c => c.Int());
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "EncEnPagDefinida", c => c.Int());
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "Mecanizado", c => c.Int());
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "Sobre", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "Sobre");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "Mecanizado");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "EncEnPagDefinida");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "Adhesivo");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "DoblezDiptico");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "Corte");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "Troquel");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "Plisado");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "PolitermolaminadoOpaco");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "PolitermolaminadoBrillante");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "BarnizGlitterSelectivo");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "BarnizUVBrillanteSelectivo");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "BarnizUVBrillante");
        }
    }
}

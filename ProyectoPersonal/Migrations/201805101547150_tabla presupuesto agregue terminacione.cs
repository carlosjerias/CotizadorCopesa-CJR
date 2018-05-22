namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tablapresupuestoagregueterminacione : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "Despacho", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "Despacho");
        }
    }
}

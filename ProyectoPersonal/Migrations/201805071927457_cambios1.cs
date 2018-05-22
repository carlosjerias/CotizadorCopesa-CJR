namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambios1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "Formato", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "Formato");
        }
    }
}

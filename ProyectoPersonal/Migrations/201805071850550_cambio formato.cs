namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambioformato : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductosEspeciales_Formato", "ProductosEspeciales_PresupuestoForm_IdPresupuesto", c => c.Int());
            CreateIndex("dbo.ProductosEspeciales_Formato", "ProductosEspeciales_PresupuestoForm_IdPresupuesto");
            AddForeignKey("dbo.ProductosEspeciales_Formato", "ProductosEspeciales_PresupuestoForm_IdPresupuesto", "dbo.ProductosEspeciales_PresupuestoForm", "IdPresupuesto");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "Formato");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "Formato", c => c.String());
            DropForeignKey("dbo.ProductosEspeciales_Formato", "ProductosEspeciales_PresupuestoForm_IdPresupuesto", "dbo.ProductosEspeciales_PresupuestoForm");
            DropIndex("dbo.ProductosEspeciales_Formato", new[] { "ProductosEspeciales_PresupuestoForm_IdPresupuesto" });
            DropColumn("dbo.ProductosEspeciales_Formato", "ProductosEspeciales_PresupuestoForm_IdPresupuesto");
        }
    }
}

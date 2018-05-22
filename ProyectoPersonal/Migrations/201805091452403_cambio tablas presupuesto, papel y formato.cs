namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambiotablaspresupuestopapelyformato : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "CostoPapel", c => c.Double(nullable: false));
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "MermaVariable", c => c.Double(nullable: false));
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "MermaFija", c => c.Double(nullable: false));
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "Kilos", c => c.Double(nullable: false));
            AddColumn("dbo.ProductosEspeciales_Papel", "ProductosEspeciales_PresupuestoForm_IdPresupuesto", c => c.Int());
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "Papel", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductosEspeciales_Papel", "ProductosEspeciales_PresupuestoForm_IdPresupuesto");
            AddForeignKey("dbo.ProductosEspeciales_Papel", "ProductosEspeciales_PresupuestoForm_IdPresupuesto", "dbo.ProductosEspeciales_PresupuestoForm", "IdPresupuesto");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductosEspeciales_Papel", "ProductosEspeciales_PresupuestoForm_IdPresupuesto", "dbo.ProductosEspeciales_PresupuestoForm");
            DropIndex("dbo.ProductosEspeciales_Papel", new[] { "ProductosEspeciales_PresupuestoForm_IdPresupuesto" });
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "Papel");
            DropColumn("dbo.ProductosEspeciales_Papel", "ProductosEspeciales_PresupuestoForm_IdPresupuesto");
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "Kilos");
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "MermaFija");
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "MermaVariable");
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "CostoPapel");
        }
    }
}

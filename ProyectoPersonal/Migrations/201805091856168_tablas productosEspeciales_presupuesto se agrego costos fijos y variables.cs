namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tablasproductosEspeciales_presupuestoseagregocostosfijosyvariables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "ImpresionCostoFijo", c => c.Double(nullable: false));
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "ImpresionCostoVariable", c => c.Double(nullable: false));
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "PapelCostoFijo", c => c.Double(nullable: false));
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "PapelCostoVariable", c => c.Double(nullable: false));
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "EncuadernacionCostoFijo", c => c.Double(nullable: false));
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "EncuadernacionCostoVariable", c => c.Double(nullable: false));
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "TerminacionCostoFijo", c => c.Double(nullable: false));
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "TerminacionCostoVariable", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "TerminacionCostoVariable");
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "TerminacionCostoFijo");
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "EncuadernacionCostoVariable");
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "EncuadernacionCostoFijo");
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "PapelCostoVariable");
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "PapelCostoFijo");
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "ImpresionCostoVariable");
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "ImpresionCostoFijo");
        }
    }
}

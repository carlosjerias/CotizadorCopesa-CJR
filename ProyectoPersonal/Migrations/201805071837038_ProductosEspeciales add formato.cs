namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductosEspecialesaddformato : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductosEspeciales_Formato",
                c => new
                    {
                        IdFormato = c.Int(nullable: false, identity: true),
                        FormatoCerradoX = c.Double(nullable: false),
                        FormatoCerradoY = c.Double(nullable: false),
                        FormatoExtendidoX = c.Double(nullable: false),
                        FormatoExtendidoY = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.IdFormato);
            
            CreateTable(
                "dbo.ProductosEspeciales_PresupuestoForm",
                c => new
                    {
                        IdPresupuesto = c.Int(nullable: false, identity: true),
                        NombrePresupuesto = c.String(),
                        Formato = c.String(),
                        Cantidad = c.Int(nullable: false),
                        Maquina = c.String(),
                        ElementosEnTamaño = c.Int(nullable: false),
                        Entradas = c.Int(nullable: false),
                        Gramaje = c.Int(nullable: false),
                        Ancho = c.Int(nullable: false),
                        Largo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdPresupuesto);
            
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "FormatoId", c => c.Int(nullable: false));
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "Formato_IdFormato", c => c.Int());
            CreateIndex("dbo.ProductosEspeciales_Presupuesto", "Formato_IdFormato");
            AddForeignKey("dbo.ProductosEspeciales_Presupuesto", "Formato_IdFormato", "dbo.ProductosEspeciales_Formato", "IdFormato");
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "Formato");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "Formato", c => c.String());
            DropForeignKey("dbo.ProductosEspeciales_Presupuesto", "Formato_IdFormato", "dbo.ProductosEspeciales_Formato");
            DropIndex("dbo.ProductosEspeciales_Presupuesto", new[] { "Formato_IdFormato" });
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "Formato_IdFormato");
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "FormatoId");
            DropTable("dbo.ProductosEspeciales_PresupuestoForm");
            DropTable("dbo.ProductosEspeciales_Formato");
        }
    }
}

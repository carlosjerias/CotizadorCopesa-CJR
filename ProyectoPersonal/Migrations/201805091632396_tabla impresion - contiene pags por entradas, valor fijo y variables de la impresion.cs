namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tablaimpresioncontienepagsporentradasvalorfijoyvariablesdelaimpresion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductosEspeciales_Impresion",
                c => new
                    {
                        ImpresionId = c.Int(nullable: false, identity: true),
                        PaginasPorEntrada = c.Int(nullable: false),
                        ValorFijo = c.Double(nullable: false),
                        ValorVariable = c.Double(nullable: false),
                        MermaFija = c.Double(nullable: false),
                        MermaVariable = c.Double(nullable: false),
                        Moneda = c.String(),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.ImpresionId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProductosEspeciales_Impresion");
        }
    }
}

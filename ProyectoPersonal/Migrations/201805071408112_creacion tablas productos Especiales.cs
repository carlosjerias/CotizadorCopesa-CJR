namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class creaciontablasproductosEspeciales : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductosEspeciales_Presupuesto",
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
                        MonedaId = c.Int(),
                        Usuarioid = c.String(nullable: false, maxLength: 128),
                        Moneda_IdMoneda = c.Int(),
                    })
                .PrimaryKey(t => t.IdPresupuesto)
                .ForeignKey("dbo.Moneda", t => t.Moneda_IdMoneda)
                .ForeignKey("dbo.AspNetUsers", t => t.Usuarioid, cascadeDelete: true)
                .Index(t => t.Usuarioid)
                .Index(t => t.Moneda_IdMoneda);
            
            CreateTable(
                "dbo.ProductosEspeciales_Presupuesto_SubProceso",
                c => new
                    {
                        IdProductoEspecial_SubProceso = c.Int(nullable: false, identity: true),
                        PresupuestoId = c.Int(nullable: false),
                        SubProcesoId = c.Int(nullable: false),
                        ValorFijo = c.Double(nullable: false),
                        ValorVariable = c.Double(nullable: false),
                        CantidadEjemplares = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdProductoEspecial_SubProceso)
                .ForeignKey("dbo.ProductosEspeciales_Presupuesto", t => t.PresupuestoId, cascadeDelete: true)
                .ForeignKey("dbo.ProductosEspeciales_SubProceso", t => t.SubProcesoId, cascadeDelete: true)
                .Index(t => t.PresupuestoId)
                .Index(t => t.SubProcesoId);
            
            CreateTable(
                "dbo.ProductosEspeciales_SubProceso",
                c => new
                    {
                        IdSubproceso = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        CostoFijo = c.Double(nullable: false),
                        CostoVariable = c.Double(nullable: false),
                        Observacion = c.String(),
                    })
                .PrimaryKey(t => t.IdSubproceso);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductosEspeciales_Presupuesto_SubProceso", "SubProcesoId", "dbo.ProductosEspeciales_SubProceso");
            DropForeignKey("dbo.ProductosEspeciales_Presupuesto_SubProceso", "PresupuestoId", "dbo.ProductosEspeciales_Presupuesto");
            DropForeignKey("dbo.ProductosEspeciales_Presupuesto", "Usuarioid", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProductosEspeciales_Presupuesto", "Moneda_IdMoneda", "dbo.Moneda");
            DropIndex("dbo.ProductosEspeciales_Presupuesto_SubProceso", new[] { "SubProcesoId" });
            DropIndex("dbo.ProductosEspeciales_Presupuesto_SubProceso", new[] { "PresupuestoId" });
            DropIndex("dbo.ProductosEspeciales_Presupuesto", new[] { "Moneda_IdMoneda" });
            DropIndex("dbo.ProductosEspeciales_Presupuesto", new[] { "Usuarioid" });
            DropTable("dbo.ProductosEspeciales_SubProceso");
            DropTable("dbo.ProductosEspeciales_Presupuesto_SubProceso");
            DropTable("dbo.ProductosEspeciales_Presupuesto");
        }
    }
}

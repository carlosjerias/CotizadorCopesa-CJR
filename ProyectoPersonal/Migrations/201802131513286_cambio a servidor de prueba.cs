namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambioaservidordeprueba : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PresupuestoForm",
                c => new
                    {
                        IdPresupuestoFormo = c.Int(nullable: false, identity: true),
                        EmpresaPapelInterior = c.Int(nullable: false),
                        EmpresaPapelTapa = c.Int(nullable: false),
                        CantidadAlzadoPlano = c.Int(nullable: false),
                        CantidadDesembolsado = c.Int(nullable: false),
                        CantidadAlzado = c.Int(nullable: false),
                        CantidadInsercion = c.Int(nullable: false),
                        CantidadPegado = c.Int(nullable: false),
                        CantidadFajado = c.Int(nullable: false),
                        CantidadPegadoSticker = c.Int(nullable: false),
                        CantidadEnCajas = c.Int(nullable: false),
                        CantidadEnZuncho = c.Int(nullable: false),
                        CantidadEnBolsa = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdPresupuestoFormo);
            
            AddColumn("dbo.TipoCatalogo", "PresupuestoForm_IdPresupuestoFormo", c => c.Int());
            AddColumn("dbo.Empresa", "PresupuestoForm_IdPresupuestoFormo", c => c.Int());
            CreateIndex("dbo.TipoCatalogo", "PresupuestoForm_IdPresupuestoFormo");
            CreateIndex("dbo.Empresa", "PresupuestoForm_IdPresupuestoFormo");
            AddForeignKey("dbo.TipoCatalogo", "PresupuestoForm_IdPresupuestoFormo", "dbo.PresupuestoForm", "IdPresupuestoFormo");
            AddForeignKey("dbo.Empresa", "PresupuestoForm_IdPresupuestoFormo", "dbo.PresupuestoForm", "IdPresupuestoFormo");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Empresa", "PresupuestoForm_IdPresupuestoFormo", "dbo.PresupuestoForm");
            DropForeignKey("dbo.TipoCatalogo", "PresupuestoForm_IdPresupuestoFormo", "dbo.PresupuestoForm");
            DropIndex("dbo.Empresa", new[] { "PresupuestoForm_IdPresupuestoFormo" });
            DropIndex("dbo.TipoCatalogo", new[] { "PresupuestoForm_IdPresupuestoFormo" });
            DropColumn("dbo.Empresa", "PresupuestoForm_IdPresupuestoFormo");
            DropColumn("dbo.TipoCatalogo", "PresupuestoForm_IdPresupuestoFormo");
            DropTable("dbo.PresupuestoForm");
        }
    }
}

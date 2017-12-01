namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambiosenPapelyCatalogo : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Papel", "MaquinaId", "dbo.Maquina");
            DropIndex("dbo.Papel", new[] { "MaquinaId" });
            CreateTable(
                "dbo.TipoCatalogo",
                c => new
                    {
                        IdTipoCatalogo = c.Int(nullable: false, identity: true),
                        NombreTipoCatalogo = c.String(),
                    })
                .PrimaryKey(t => t.IdTipoCatalogo);
            
            AddColumn("dbo.Presupuesto", "TipoCatalogoId", c => c.Int(nullable: false));
            AddColumn("dbo.Interior", "MaquinaId", c => c.Int());
            AddColumn("dbo.Tapa", "MaquinaId", c => c.Int());
            AddColumn("dbo.Presupuesto_SubProceso", "ValorTotalSubProceso", c => c.Double(nullable: false));
            CreateIndex("dbo.Presupuesto", "TipoCatalogoId");
            CreateIndex("dbo.Interior", "MaquinaId");
            CreateIndex("dbo.Tapa", "MaquinaId");
            AddForeignKey("dbo.Interior", "MaquinaId", "dbo.Maquina", "IdMaquina");
            AddForeignKey("dbo.Tapa", "MaquinaId", "dbo.Maquina", "IdMaquina");
            AddForeignKey("dbo.Presupuesto", "TipoCatalogoId", "dbo.TipoCatalogo", "IdTipoCatalogo", cascadeDelete: true);
            DropColumn("dbo.Papel", "MaquinaId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Papel", "MaquinaId", c => c.Int());
            DropForeignKey("dbo.Presupuesto", "TipoCatalogoId", "dbo.TipoCatalogo");
            DropForeignKey("dbo.Tapa", "MaquinaId", "dbo.Maquina");
            DropForeignKey("dbo.Interior", "MaquinaId", "dbo.Maquina");
            DropIndex("dbo.Tapa", new[] { "MaquinaId" });
            DropIndex("dbo.Interior", new[] { "MaquinaId" });
            DropIndex("dbo.Presupuesto", new[] { "TipoCatalogoId" });
            DropColumn("dbo.Presupuesto_SubProceso", "ValorTotalSubProceso");
            DropColumn("dbo.Tapa", "MaquinaId");
            DropColumn("dbo.Interior", "MaquinaId");
            DropColumn("dbo.Presupuesto", "TipoCatalogoId");
            DropTable("dbo.TipoCatalogo");
            CreateIndex("dbo.Papel", "MaquinaId");
            AddForeignKey("dbo.Papel", "MaquinaId", "dbo.Maquina", "IdMaquina");
        }
    }
}

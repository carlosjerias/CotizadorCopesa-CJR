namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PapelEmpresa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Papel", "EmpresaId", c => c.Int(nullable: false));
            CreateIndex("dbo.Papel", "EmpresaId");
            AddForeignKey("dbo.Papel", "EmpresaId", "dbo.Empresa", "IdEmpresa", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Papel", "EmpresaId", "dbo.Empresa");
            DropIndex("dbo.Papel", new[] { "EmpresaId" });
            DropColumn("dbo.Papel", "EmpresaId");
        }
    }
}

namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cambiobasededatosinterioropcionalencuadernacionopcional : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Presupuesto", "InteriorId", "dbo.Interior");
            DropIndex("dbo.Presupuesto", new[] { "InteriorId" });
            AlterColumn("dbo.Presupuesto", "InteriorId", c => c.Int());
            CreateIndex("dbo.Presupuesto", "InteriorId");
            AddForeignKey("dbo.Presupuesto", "InteriorId", "dbo.Interior", "IdInterior");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Presupuesto", "InteriorId", "dbo.Interior");
            DropIndex("dbo.Presupuesto", new[] { "InteriorId" });
            AlterColumn("dbo.Presupuesto", "InteriorId", c => c.Int(nullable: false));
            CreateIndex("dbo.Presupuesto", "InteriorId");
            AddForeignKey("dbo.Presupuesto", "InteriorId", "dbo.Interior", "IdInterior", cascadeDelete: true);
        }
    }
}

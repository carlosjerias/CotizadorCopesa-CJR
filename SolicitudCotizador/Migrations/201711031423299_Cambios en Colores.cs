namespace SolicitudCotizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambiosenColores : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Colores", new[] { "Proceso_IdProceso" });
            DropColumn("dbo.Colores", "ProcesoId");
            RenameColumn(table: "dbo.Colores", name: "Proceso_IdProceso", newName: "ProcesoId");
            AlterColumn("dbo.Colores", "ProcesoId", c => c.Int());
            CreateIndex("dbo.Colores", "ProcesoId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Colores", new[] { "ProcesoId" });
            AlterColumn("dbo.Colores", "ProcesoId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Colores", name: "ProcesoId", newName: "Proceso_IdProceso");
            AddColumn("dbo.Colores", "ProcesoId", c => c.Int(nullable: false));
            CreateIndex("dbo.Colores", "Proceso_IdProceso");
        }
    }
}

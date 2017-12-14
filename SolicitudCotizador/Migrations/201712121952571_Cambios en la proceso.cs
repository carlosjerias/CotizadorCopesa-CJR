namespace SolicitudCotizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cambiosenlaproceso : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Proceso", "SolicitudId", "dbo.Solicitud");
            DropIndex("dbo.Proceso", new[] { "SolicitudId" });
            AlterColumn("dbo.Proceso", "SolicitudId", c => c.Int());
            CreateIndex("dbo.Proceso", "SolicitudId");
            AddForeignKey("dbo.Proceso", "SolicitudId", "dbo.Solicitud", "IdSolicitud");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Proceso", "SolicitudId", "dbo.Solicitud");
            DropIndex("dbo.Proceso", new[] { "SolicitudId" });
            AlterColumn("dbo.Proceso", "SolicitudId", c => c.Int(nullable: false));
            CreateIndex("dbo.Proceso", "SolicitudId");
            AddForeignKey("dbo.Proceso", "SolicitudId", "dbo.Solicitud", "IdSolicitud", cascadeDelete: true);
        }
    }
}

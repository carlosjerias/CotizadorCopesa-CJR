namespace SolicitudCotizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Seagregodespacho : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Despacho",
                c => new
                    {
                        DespachoId = c.Int(nullable: false, identity: true),
                        NombreDespacho = c.String(),
                    })
                .PrimaryKey(t => t.DespachoId);
            
            AddColumn("dbo.Solicitud", "DespachoId", c => c.Int(nullable: false));
            CreateIndex("dbo.Solicitud", "DespachoId");
            AddForeignKey("dbo.Solicitud", "DespachoId", "dbo.Despacho", "DespachoId", cascadeDelete: true);
            DropColumn("dbo.Solicitud", "Despacho");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Solicitud", "Despacho", c => c.String());
            DropForeignKey("dbo.Solicitud", "DespachoId", "dbo.Despacho");
            DropIndex("dbo.Solicitud", new[] { "DespachoId" });
            DropColumn("dbo.Solicitud", "DespachoId");
            DropTable("dbo.Despacho");
        }
    }
}

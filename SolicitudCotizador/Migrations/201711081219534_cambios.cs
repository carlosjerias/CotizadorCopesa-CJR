namespace SolicitudCotizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambios : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cliente",
                c => new
                    {
                        IdCliente = c.Int(nullable: false, identity: true),
                        RutCliente = c.String(),
                        CodCliente = c.Int(nullable: false),
                        NombreCliente = c.String(),
                        ContactoCliente = c.String(),
                        EmailCliente = c.String(),
                        Telefono = c.String(),
                    })
                .PrimaryKey(t => t.IdCliente);
            
            AddColumn("dbo.Solicitud", "ClienteId", c => c.Int(nullable: false));
            CreateIndex("dbo.Solicitud", "ClienteId");
            AddForeignKey("dbo.Solicitud", "ClienteId", "dbo.Cliente", "IdCliente", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Solicitud", "ClienteId", "dbo.Cliente");
            DropIndex("dbo.Solicitud", new[] { "ClienteId" });
            DropColumn("dbo.Solicitud", "ClienteId");
            DropTable("dbo.Cliente");
        }
    }
}

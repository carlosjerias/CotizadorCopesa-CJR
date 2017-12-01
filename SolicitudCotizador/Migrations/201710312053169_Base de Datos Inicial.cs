namespace SolicitudCotizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BasedeDatosInicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Colores",
                c => new
                    {
                        IdColores = c.Int(nullable: false, identity: true),
                        NumeroColor = c.Int(nullable: false),
                        ProcesoId = c.Int(nullable: false),
                        Proceso_IdProceso = c.Int(),
                    })
                .PrimaryKey(t => t.IdColores)
                .ForeignKey("dbo.Proceso", t => t.Proceso_IdProceso)
                .Index(t => t.Proceso_IdProceso);
            
            CreateTable(
                "dbo.Proceso",
                c => new
                    {
                        IdProceso = c.Int(nullable: false, identity: true),
                        NombreProceso = c.String(),
                        CantidadProceso = c.Int(nullable: false),
                        Papel = c.String(),
                        Gramaje = c.Int(nullable: false),
                        Observacion = c.String(),
                        SolicitudId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdProceso)
                .ForeignKey("dbo.Solicitud", t => t.SolicitudId, cascadeDelete: true)
                .Index(t => t.SolicitudId);
            
            CreateTable(
                "dbo.Solicitud",
                c => new
                    {
                        IdSolicitud = c.Int(nullable: false, identity: true),
                        NombreProducto = c.String(),
                        FormatoId = c.Int(nullable: false),
                        EncuadernacionId = c.Int(nullable: false),
                        FechaProduccion = c.DateTime(nullable: false),
                        Embalaje = c.String(),
                        Despacho = c.String(),
                        CantidadPaginasTotales = c.Int(nullable: false),
                        ColoresTotales = c.String(),
                    })
                .PrimaryKey(t => t.IdSolicitud)
                .ForeignKey("dbo.Encuadernacion", t => t.EncuadernacionId, cascadeDelete: true)
                .ForeignKey("dbo.Formato", t => t.FormatoId, cascadeDelete: true)
                .Index(t => t.FormatoId)
                .Index(t => t.EncuadernacionId);
            
            CreateTable(
                "dbo.Encuadernacion",
                c => new
                    {
                        IdEncuadernacion = c.Int(nullable: false, identity: true),
                        NombreEncuadernacion = c.String(),
                    })
                .PrimaryKey(t => t.IdEncuadernacion);
            
            CreateTable(
                "dbo.Formato",
                c => new
                    {
                        IdFormato = c.Int(nullable: false, identity: true),
                        FormatoCerradoX = c.Double(nullable: false),
                        FormatoCerradoY = c.Double(nullable: false),
                        FormatoExtendidoX = c.Double(nullable: false),
                        FormatoExtendidoY = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.IdFormato);
            
            CreateTable(
                "dbo.Tiraje",
                c => new
                    {
                        IdTiraje = c.Int(nullable: false, identity: true),
                        Cantidad = c.Int(nullable: false),
                        SolicitudId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdTiraje)
                .ForeignKey("dbo.Solicitud", t => t.SolicitudId, cascadeDelete: true)
                .Index(t => t.SolicitudId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Proceso", "SolicitudId", "dbo.Solicitud");
            DropForeignKey("dbo.Tiraje", "SolicitudId", "dbo.Solicitud");
            DropForeignKey("dbo.Solicitud", "FormatoId", "dbo.Formato");
            DropForeignKey("dbo.Solicitud", "EncuadernacionId", "dbo.Encuadernacion");
            DropForeignKey("dbo.Colores", "Proceso_IdProceso", "dbo.Proceso");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Tiraje", new[] { "SolicitudId" });
            DropIndex("dbo.Solicitud", new[] { "EncuadernacionId" });
            DropIndex("dbo.Solicitud", new[] { "FormatoId" });
            DropIndex("dbo.Proceso", new[] { "SolicitudId" });
            DropIndex("dbo.Colores", new[] { "Proceso_IdProceso" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Tiraje");
            DropTable("dbo.Formato");
            DropTable("dbo.Encuadernacion");
            DropTable("dbo.Solicitud");
            DropTable("dbo.Proceso");
            DropTable("dbo.Colores");
        }
    }
}

namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambiosenFormatoyPapel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Formato", "Interior_Ancho", c => c.Double(nullable: false));
            AddColumn("dbo.Formato", "Interior_Alto", c => c.Double(nullable: false));
            AddColumn("dbo.Formato", "TapaDiptica_Ancho", c => c.Double(nullable: false));
            AddColumn("dbo.Formato", "TapaDiptica_Alto", c => c.Double(nullable: false));
            AddColumn("dbo.Formato", "TapaTriptica_Ancho_", c => c.Double(nullable: false));
            AddColumn("dbo.Formato", "TapaTriptica_Alto", c => c.Double(nullable: false));
            AddColumn("dbo.Maquina", "TipoMaquina", c => c.String());
            DropColumn("dbo.Papel", "AnchoBanda");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Papel", "AnchoBanda", c => c.Double(nullable: false));
            DropColumn("dbo.Maquina", "TipoMaquina");
            DropColumn("dbo.Formato", "TapaTriptica_Alto");
            DropColumn("dbo.Formato", "TapaTriptica_Ancho_");
            DropColumn("dbo.Formato", "TapaDiptica_Alto");
            DropColumn("dbo.Formato", "TapaDiptica_Ancho");
            DropColumn("dbo.Formato", "Interior_Alto");
            DropColumn("dbo.Formato", "Interior_Ancho");
        }
    }
}

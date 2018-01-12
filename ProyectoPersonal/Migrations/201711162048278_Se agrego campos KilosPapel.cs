namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeagregocamposKilosPapel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Interior", "KilosPapel", c => c.Single(nullable: false));
            AddColumn("dbo.Interior", "Entradas", c => c.Single(nullable: false));
            AddColumn("dbo.Interior", "Tiradas", c => c.Single(nullable: false));
            AddColumn("dbo.Tapa", "KilosPapel", c => c.Single(nullable: false));
            AddColumn("dbo.Tapa", "Entradas", c => c.Single(nullable: false));
            AddColumn("dbo.Tapa", "Tiradas", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tapa", "Tiradas");
            DropColumn("dbo.Tapa", "Entradas");
            DropColumn("dbo.Tapa", "KilosPapel");
            DropColumn("dbo.Interior", "Tiradas");
            DropColumn("dbo.Interior", "Entradas");
            DropColumn("dbo.Interior", "KilosPapel");
        }
    }
}

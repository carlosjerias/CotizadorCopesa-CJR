namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SecambioacatalogodeEmbalaje : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TipoCatalogo", "DimensionesCajasStandar", c => c.String());
            AddColumn("dbo.TipoCatalogo", "DimensionesCajasSachet", c => c.String());
            DropColumn("dbo.Embalaje", "DimensionesCajasStandar");
            DropColumn("dbo.Embalaje", "DimensionesCajasSachet");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Embalaje", "DimensionesCajasSachet", c => c.String());
            AddColumn("dbo.Embalaje", "DimensionesCajasStandar", c => c.String());
            DropColumn("dbo.TipoCatalogo", "DimensionesCajasSachet");
            DropColumn("dbo.TipoCatalogo", "DimensionesCajasStandar");
        }
    }
}

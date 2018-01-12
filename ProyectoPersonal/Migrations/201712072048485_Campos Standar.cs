namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CamposStandar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Embalaje", "DimensionesCajasStandar", c => c.String());
            AddColumn("dbo.Embalaje", "DimensionesCajasSachet", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Embalaje", "DimensionesCajasSachet");
            DropColumn("dbo.Embalaje", "DimensionesCajasStandar");
        }
    }
}

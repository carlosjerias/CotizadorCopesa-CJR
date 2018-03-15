namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agregandocomovariablesnormales : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TipoCatalogo", "PapelInteriorId", c => c.Int());
            AddColumn("dbo.TipoCatalogo", "PapelTapaId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TipoCatalogo", "PapelTapaId");
            DropColumn("dbo.TipoCatalogo", "PapelInteriorId");
        }
    }
}

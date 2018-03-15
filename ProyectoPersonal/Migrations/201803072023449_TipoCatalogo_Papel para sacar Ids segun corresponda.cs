namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TipoCatalogo_PapelparasacarIdsseguncorresponda : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Papel", "TipoCatalogo_IdTipoCatalogo", c => c.Int());
            CreateIndex("dbo.Papel", "TipoCatalogo_IdTipoCatalogo");
            AddForeignKey("dbo.Papel", "TipoCatalogo_IdTipoCatalogo", "dbo.TipoCatalogo", "IdTipoCatalogo");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Papel", "TipoCatalogo_IdTipoCatalogo", "dbo.TipoCatalogo");
            DropIndex("dbo.Papel", new[] { "TipoCatalogo_IdTipoCatalogo" });
            DropColumn("dbo.Papel", "TipoCatalogo_IdTipoCatalogo");
        }
    }
}

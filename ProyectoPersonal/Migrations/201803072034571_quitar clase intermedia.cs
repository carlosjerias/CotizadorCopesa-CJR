namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class quitarclaseintermedia : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Papel", "TipoCatalogo_IdTipoCatalogo", "dbo.TipoCatalogo");
            DropIndex("dbo.Papel", new[] { "TipoCatalogo_IdTipoCatalogo" });
            DropColumn("dbo.Papel", "TipoCatalogo_IdTipoCatalogo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Papel", "TipoCatalogo_IdTipoCatalogo", c => c.Int());
            CreateIndex("dbo.Papel", "TipoCatalogo_IdTipoCatalogo");
            AddForeignKey("dbo.Papel", "TipoCatalogo_IdTipoCatalogo", "dbo.TipoCatalogo", "IdTipoCatalogo");
        }
    }
}

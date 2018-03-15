namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class quitandofk : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TipoCatalogo", "PapelIntId_IdPapel", "dbo.Papel");
            DropForeignKey("dbo.TipoCatalogo", "PapelTapaId_IdPapel", "dbo.Papel");
            DropIndex("dbo.TipoCatalogo", new[] { "PapelIntId_IdPapel" });
            DropIndex("dbo.TipoCatalogo", new[] { "PapelTapaId_IdPapel" });
            DropColumn("dbo.TipoCatalogo", "PapelIntId_IdPapel");
            DropColumn("dbo.TipoCatalogo", "PapelTapaId_IdPapel");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TipoCatalogo", "PapelTapaId_IdPapel", c => c.Int());
            AddColumn("dbo.TipoCatalogo", "PapelIntId_IdPapel", c => c.Int());
            CreateIndex("dbo.TipoCatalogo", "PapelTapaId_IdPapel");
            CreateIndex("dbo.TipoCatalogo", "PapelIntId_IdPapel");
            AddForeignKey("dbo.TipoCatalogo", "PapelTapaId_IdPapel", "dbo.Papel", "IdPapel");
            AddForeignKey("dbo.TipoCatalogo", "PapelIntId_IdPapel", "dbo.Papel", "IdPapel");
        }
    }
}

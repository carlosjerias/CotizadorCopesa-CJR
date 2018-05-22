namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Papelhoja : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductosEspeciales_Papel",
                c => new
                    {
                        IdPapel = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Gramaje = c.Int(nullable: false),
                        Ancho = c.Int(nullable: false),
                        Largo = c.Int(nullable: false),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.IdPapel);
            
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "PapelId", c => c.Int(nullable: false));
            AddColumn("dbo.ProductosEspeciales_Presupuesto", "Papel_IdPapel", c => c.Int());
            CreateIndex("dbo.ProductosEspeciales_Presupuesto", "Papel_IdPapel");
            AddForeignKey("dbo.ProductosEspeciales_Presupuesto", "Papel_IdPapel", "dbo.ProductosEspeciales_Papel", "IdPapel");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductosEspeciales_Presupuesto", "Papel_IdPapel", "dbo.ProductosEspeciales_Papel");
            DropIndex("dbo.ProductosEspeciales_Presupuesto", new[] { "Papel_IdPapel" });
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "Papel_IdPapel");
            DropColumn("dbo.ProductosEspeciales_Presupuesto", "PapelId");
            DropTable("dbo.ProductosEspeciales_Papel");
        }
    }
}

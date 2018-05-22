namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tablaimpresionagreguemaquina : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductosEspeciales_Impresion", "Maquina_IdMaquina", c => c.Int());
            CreateIndex("dbo.ProductosEspeciales_Impresion", "Maquina_IdMaquina");
            AddForeignKey("dbo.ProductosEspeciales_Impresion", "Maquina_IdMaquina", "dbo.Maquina", "IdMaquina");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductosEspeciales_Impresion", "Maquina_IdMaquina", "dbo.Maquina");
            DropIndex("dbo.ProductosEspeciales_Impresion", new[] { "Maquina_IdMaquina" });
            DropColumn("dbo.ProductosEspeciales_Impresion", "Maquina_IdMaquina");
        }
    }
}

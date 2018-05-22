namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nombreFormato : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductosEspeciales_Formato", "NombreFormato", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductosEspeciales_Formato", "NombreFormato");
        }
    }
}

namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modelopapelagreguevalorkilosdepapel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductosEspeciales_Papel", "PrecioKilos", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductosEspeciales_Papel", "PrecioKilos");
        }
    }
}

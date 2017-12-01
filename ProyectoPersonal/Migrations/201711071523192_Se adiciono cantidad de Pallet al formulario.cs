namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeadicionocantidaddePalletalformulario : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Presupuesto", "CantidadPallet", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Presupuesto", "CantidadPallet");
        }
    }
}

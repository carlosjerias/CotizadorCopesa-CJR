namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FaltounCampo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Presupuesto", "CantidadModelos", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Presupuesto", "CantidadModelos");
        }
    }
}

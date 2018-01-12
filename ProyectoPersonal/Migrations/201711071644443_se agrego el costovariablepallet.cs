namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seagregoelcostovariablepallet : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Presupuesto", "CostoVariablePallet", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Presupuesto", "CostoVariablePallet");
        }
    }
}

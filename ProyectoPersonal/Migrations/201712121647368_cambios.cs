namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambios : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Presupuesto", "CostoVariablePallet", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Presupuesto", "CostoVariablePallet", c => c.Int(nullable: false));
        }
    }
}

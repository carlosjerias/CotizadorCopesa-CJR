namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class campoparabarnizacuosoydistribucion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Presupuesto", "BarnizAcuoso", c => c.Int(nullable: false));
            AddColumn("dbo.Presupuesto", "CantEnCajas", c => c.Int(nullable: false));
            AddColumn("dbo.Presupuesto", "CantEnZuncho", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Presupuesto", "CantEnZuncho");
            DropColumn("dbo.Presupuesto", "CantEnCajas");
            DropColumn("dbo.Presupuesto", "BarnizAcuoso");
        }
    }
}

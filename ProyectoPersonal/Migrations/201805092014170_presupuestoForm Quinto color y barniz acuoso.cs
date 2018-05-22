namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class presupuestoFormQuintocolorybarnizacuoso : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "QuintoColorPMS", c => c.Int(nullable: false));
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "QuintoColorFluor", c => c.Int(nullable: false));
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "QuintoColorMetalico", c => c.Int(nullable: false));
            AddColumn("dbo.ProductosEspeciales_PresupuestoForm", "BarnizAcuoso", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "BarnizAcuoso");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "QuintoColorMetalico");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "QuintoColorFluor");
            DropColumn("dbo.ProductosEspeciales_PresupuestoForm", "QuintoColorPMS");
        }
    }
}

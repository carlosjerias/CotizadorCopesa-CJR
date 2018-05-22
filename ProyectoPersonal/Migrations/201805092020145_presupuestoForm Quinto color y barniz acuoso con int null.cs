namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class presupuestoFormQuintocolorybarnizacuosoconintnull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductosEspeciales_PresupuestoForm", "QuintoColorPMS", c => c.Int());
            AlterColumn("dbo.ProductosEspeciales_PresupuestoForm", "QuintoColorFluor", c => c.Int());
            AlterColumn("dbo.ProductosEspeciales_PresupuestoForm", "QuintoColorMetalico", c => c.Int());
            AlterColumn("dbo.ProductosEspeciales_PresupuestoForm", "BarnizAcuoso", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductosEspeciales_PresupuestoForm", "BarnizAcuoso", c => c.Int(nullable: false));
            AlterColumn("dbo.ProductosEspeciales_PresupuestoForm", "QuintoColorMetalico", c => c.Int(nullable: false));
            AlterColumn("dbo.ProductosEspeciales_PresupuestoForm", "QuintoColorFluor", c => c.Int(nullable: false));
            AlterColumn("dbo.ProductosEspeciales_PresupuestoForm", "QuintoColorPMS", c => c.Int(nullable: false));
        }
    }
}

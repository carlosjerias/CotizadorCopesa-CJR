namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seadicionoelcampocantidadpres_subPRoceso : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Presupuesto_SubProceso", "CantidadEjemplaresProceso", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Presupuesto_SubProceso", "CantidadEjemplaresProceso");
        }
    }
}

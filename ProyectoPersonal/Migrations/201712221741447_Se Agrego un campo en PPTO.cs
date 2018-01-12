namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeAgregouncampoenPPTO : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Presupuesto", "CantidadTerminacionEmbolsado", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Presupuesto", "CantidadTerminacionEmbolsado");
        }
    }
}

namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Agreguecampoestadoalpresupuesto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Presupuesto", "Estado", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Presupuesto", "Estado");
        }
    }
}

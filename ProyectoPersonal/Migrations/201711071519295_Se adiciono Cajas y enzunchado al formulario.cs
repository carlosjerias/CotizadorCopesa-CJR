namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeadicionoCajasyenzunchadoalformulario : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Presupuesto", "CantidadCajas", c => c.Int(nullable: false));
            AddColumn("dbo.Presupuesto", "Enzunchadoxpqte", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Presupuesto", "Enzunchadoxpqte");
            DropColumn("dbo.Presupuesto", "CantidadCajas");
        }
    }
}

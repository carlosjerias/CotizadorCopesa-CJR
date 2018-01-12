namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seagregoFormatoSeleccionadoaCatalogo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TipoCatalogo", "FormatoSeleccionado", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TipoCatalogo", "FormatoSeleccionado");
        }
    }
}

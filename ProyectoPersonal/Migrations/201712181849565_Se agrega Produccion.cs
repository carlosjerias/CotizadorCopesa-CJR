namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeagregaProduccion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Produccion",
                c => new
                    {
                        IdProduccion = c.Int(nullable: false, identity: true),
                        Paginas = c.Int(nullable: false),
                        Litho132cms = c.Double(nullable: false),
                        Litho174cms = c.Double(nullable: false),
                        Web88cms = c.Double(nullable: false),
                        Impresion64 = c.Int(nullable: false),
                        Impresion48 = c.Int(nullable: false),
                        Impresion32 = c.Int(nullable: false),
                        Impresion24 = c.Int(nullable: false),
                        Impresion16 = c.Int(nullable: false),
                        Impresion08 = c.Int(nullable: false),
                        Impresion04 = c.Int(nullable: false),
                        Entradas64 = c.Int(nullable: false),
                        Entradas48 = c.Int(nullable: false),
                        Entradas16 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdProduccion);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Produccion");
        }
    }
}

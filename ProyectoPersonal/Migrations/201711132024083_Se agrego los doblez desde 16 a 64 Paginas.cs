namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Seagregolosdoblezdesde16a64Paginas : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Presupuesto", "EntradasPag64", c => c.Int(nullable: false));
            AddColumn("dbo.Presupuesto", "EntradasPag48", c => c.Int(nullable: false));
            AddColumn("dbo.Presupuesto", "EntradasPag32", c => c.Int(nullable: false));
            AddColumn("dbo.Presupuesto", "EntradasPag24", c => c.Int(nullable: false));
            AddColumn("dbo.Presupuesto", "CostoFijoPag64", c => c.Double(nullable: false));
            AddColumn("dbo.Presupuesto", "CostoFijoPag48", c => c.Double(nullable: false));
            AddColumn("dbo.Presupuesto", "CostoFijoPag32", c => c.Double(nullable: false));
            AddColumn("dbo.Presupuesto", "CostoFijoPag24", c => c.Double(nullable: false));
            AddColumn("dbo.Presupuesto", "CostoVariablePag64", c => c.Double(nullable: false));
            AddColumn("dbo.Presupuesto", "CostoVariablePag48", c => c.Double(nullable: false));
            AddColumn("dbo.Presupuesto", "CostoVariablePag32", c => c.Double(nullable: false));
            AddColumn("dbo.Presupuesto", "CostoVariablePag24", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Presupuesto", "CostoVariablePag24");
            DropColumn("dbo.Presupuesto", "CostoVariablePag32");
            DropColumn("dbo.Presupuesto", "CostoVariablePag48");
            DropColumn("dbo.Presupuesto", "CostoVariablePag64");
            DropColumn("dbo.Presupuesto", "CostoFijoPag24");
            DropColumn("dbo.Presupuesto", "CostoFijoPag32");
            DropColumn("dbo.Presupuesto", "CostoFijoPag48");
            DropColumn("dbo.Presupuesto", "CostoFijoPag64");
            DropColumn("dbo.Presupuesto", "EntradasPag24");
            DropColumn("dbo.Presupuesto", "EntradasPag32");
            DropColumn("dbo.Presupuesto", "EntradasPag48");
            DropColumn("dbo.Presupuesto", "EntradasPag64");
        }
    }
}

namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeadicionoPalletenEmbalaje : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Embalaje", "PalletEstandar", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Embalaje", "PalletEstandar");
        }
    }
}

namespace ProyectoPersonal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BasededatosInicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Doblez",
                c => new
                    {
                        IdDoblez = c.Int(nullable: false, identity: true),
                        NombreDoblez = c.String(),
                        NroDoblez = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdDoblez);
            
            CreateTable(
                "dbo.Formato",
                c => new
                    {
                        IdFormato = c.Int(nullable: false, identity: true),
                        FormatoCerradoX = c.Double(nullable: false),
                        FormatoCerradoY = c.Double(nullable: false),
                        FormatoExtendidoX = c.Double(nullable: false),
                        FormatoExtendidoY = c.Double(nullable: false),
                        EntradasxFormatos = c.Int(nullable: false),
                        DoblezId = c.Int(),
                    })
                .PrimaryKey(t => t.IdFormato)
                .ForeignKey("dbo.Doblez", t => t.DoblezId)
                .Index(t => t.DoblezId);
            
            CreateTable(
                "dbo.Presupuesto",
                c => new
                    {
                        IdPresupuesto = c.Int(nullable: false, identity: true),
                        NombrePresupuesto = c.String(),
                        Tiraje = c.Int(nullable: false),
                        FormatoId = c.Int(),
                        EncuadernacionId = c.Int(),
                        InteriorId = c.Int(nullable: false),
                        TapaId = c.Int(),
                        TotalNetoFijo = c.Double(nullable: false),
                        TotalNetoVari = c.Double(nullable: false),
                        TotalNetoTotal = c.Double(nullable: false),
                        PrecioUnitario = c.Double(nullable: false),
                        MonedaId = c.Int(nullable: false),
                        EntradasPag16 = c.Int(nullable: false),
                        EntradasPag12 = c.Int(nullable: false),
                        EntradasPag8 = c.Int(nullable: false),
                        EntradasPag4 = c.Int(nullable: false),
                        CostoFijoPag16 = c.Double(nullable: false),
                        CostoFijoPag12 = c.Double(nullable: false),
                        CostoFijoPag8 = c.Double(nullable: false),
                        CostoFijoPag4 = c.Double(nullable: false),
                        CostoVariablePag16 = c.Double(nullable: false),
                        CostoVariablePag12 = c.Double(nullable: false),
                        CostoVariablePag8 = c.Double(nullable: false),
                        CostoVariablePag4 = c.Double(nullable: false),
                        CostoFijoEncuadernacion = c.Double(nullable: false),
                        CostoVariableEncuadernacion = c.Double(nullable: false),
                        CostoFijoTapa = c.Double(nullable: false),
                        CostoVariableTapa = c.Double(nullable: false),
                        TarifaFijaImpresion = c.Double(nullable: false),
                        TarifaVariableImpresion = c.Double(nullable: false),
                        TarifaFijaPapel = c.Double(nullable: false),
                        TarifaVariablePapel = c.Double(nullable: false),
                        TarifaFijaEncuadernacion = c.Double(nullable: false),
                        TarifaVariableEncuadernacion = c.Double(nullable: false),
                        TarifaFijaTerminacion = c.Double(nullable: false),
                        TarifaVariableTerminacion = c.Double(nullable: false),
                        FechaCreacion = c.DateTime(nullable: false),
                        Usuarioid = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.IdPresupuesto)
                .ForeignKey("dbo.Encuadernacion", t => t.EncuadernacionId)
                .ForeignKey("dbo.Formato", t => t.FormatoId)
                .ForeignKey("dbo.Interior", t => t.InteriorId, cascadeDelete: true)
                .ForeignKey("dbo.Moneda", t => t.MonedaId, cascadeDelete: true)
                .ForeignKey("dbo.Tapa", t => t.TapaId)
                .ForeignKey("dbo.AspNetUsers", t => t.Usuarioid, cascadeDelete: true)
                .Index(t => t.FormatoId)
                .Index(t => t.EncuadernacionId)
                .Index(t => t.InteriorId)
                .Index(t => t.TapaId)
                .Index(t => t.MonedaId)
                .Index(t => t.Usuarioid);
            
            CreateTable(
                "dbo.Encuadernacion",
                c => new
                    {
                        IdEncuadernacion = c.Int(nullable: false, identity: true),
                        NombreEncuadernacion = c.String(),
                        ValorFijo = c.Double(nullable: false),
                        ValorVariable = c.Double(nullable: false),
                        TipoMonedaId = c.Int(),
                        DescripcionEnc = c.String(),
                    })
                .PrimaryKey(t => t.IdEncuadernacion)
                .ForeignKey("dbo.TipoMoneda", t => t.TipoMonedaId)
                .Index(t => t.TipoMonedaId);
            
            CreateTable(
                "dbo.TipoMoneda",
                c => new
                    {
                        IdTipoMoneda = c.Int(nullable: false, identity: true),
                        NombreTipoMoneda = c.String(),
                        SiglasTipoMoneda = c.String(),
                    })
                .PrimaryKey(t => t.IdTipoMoneda);
            
            CreateTable(
                "dbo.Impresion",
                c => new
                    {
                        IdImpresion = c.Int(nullable: false, identity: true),
                        NombreImpresion = c.String(),
                        ValorFijoImpresion = c.Double(nullable: false),
                        valorvariableImpresion = c.Double(nullable: false),
                        MaquinaId = c.Int(),
                        TipoMonedaId = c.Int(),
                    })
                .PrimaryKey(t => t.IdImpresion)
                .ForeignKey("dbo.Maquina", t => t.MaquinaId)
                .ForeignKey("dbo.TipoMoneda", t => t.TipoMonedaId)
                .Index(t => t.MaquinaId)
                .Index(t => t.TipoMonedaId);
            
            CreateTable(
                "dbo.Maquina",
                c => new
                    {
                        IdMaquina = c.Int(nullable: false, identity: true),
                        NombreMaquina = c.String(),
                        MermaFija = c.Int(nullable: false),
                        MermaVariable = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.IdMaquina);
            
            CreateTable(
                "dbo.Papel",
                c => new
                    {
                        IdPapel = c.Int(nullable: false, identity: true),
                        NombrePapel = c.String(),
                        Gramaje = c.Int(nullable: false),
                        AnchoBanda = c.Int(nullable: false),
                        MaquinaId = c.Int(),
                        Lomo = c.Double(nullable: false),
                        Adhesivo = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.IdPapel)
                .ForeignKey("dbo.Maquina", t => t.MaquinaId)
                .Index(t => t.MaquinaId);
            
            CreateTable(
                "dbo.Interior",
                c => new
                    {
                        IdInterior = c.Int(nullable: false, identity: true),
                        CantidadPaginas = c.Int(nullable: false),
                        PapelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdInterior)
                .ForeignKey("dbo.Papel", t => t.PapelId, cascadeDelete: true)
                .Index(t => t.PapelId);
            
            CreateTable(
                "dbo.Tapa",
                c => new
                    {
                        IdTapa = c.Int(nullable: false, identity: true),
                        CantidadPaginas = c.Int(nullable: false),
                        PapelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdTapa)
                .ForeignKey("dbo.Papel", t => t.PapelId, cascadeDelete: true)
                .Index(t => t.PapelId);
            
            CreateTable(
                "dbo.Moneda",
                c => new
                    {
                        IdMoneda = c.Int(nullable: false, identity: true),
                        TipoMonedaId = c.Int(nullable: false),
                        Valor = c.Double(nullable: false),
                        FechaValor = c.DateTime(nullable: false),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdMoneda)
                .ForeignKey("dbo.TipoMoneda", t => t.TipoMonedaId, cascadeDelete: true)
                .Index(t => t.TipoMonedaId);
            
            CreateTable(
                "dbo.SubProceso",
                c => new
                    {
                        IdSubProceso = c.Int(nullable: false, identity: true),
                        NombreSubProceso = c.String(nullable: false),
                        CostoFijoSubProceso = c.Double(nullable: false),
                        CostoVariableSubProceso = c.Double(nullable: false),
                        Observacion = c.String(),
                        ProcesoId = c.Int(nullable: false),
                        DoblezId = c.Int(),
                        UnidadMedidaId = c.Int(nullable: false),
                        TipoMonedaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdSubProceso)
                .ForeignKey("dbo.Doblez", t => t.DoblezId)
                .ForeignKey("dbo.Proceso", t => t.ProcesoId, cascadeDelete: true)
                .ForeignKey("dbo.TipoMoneda", t => t.TipoMonedaId, cascadeDelete: true)
                .ForeignKey("dbo.UnidadMedida", t => t.UnidadMedidaId, cascadeDelete: true)
                .Index(t => t.ProcesoId)
                .Index(t => t.DoblezId)
                .Index(t => t.UnidadMedidaId)
                .Index(t => t.TipoMonedaId);
            
            CreateTable(
                "dbo.Proceso",
                c => new
                    {
                        IdProceso = c.Int(nullable: false, identity: true),
                        NombreProceso = c.String(),
                        TipoProcesoId = c.Int(),
                    })
                .PrimaryKey(t => t.IdProceso)
                .ForeignKey("dbo.TipoProceso", t => t.TipoProcesoId)
                .Index(t => t.TipoProcesoId);
            
            CreateTable(
                "dbo.TipoProceso",
                c => new
                    {
                        IdTipoProceso = c.Int(nullable: false, identity: true),
                        NombreTipoProceso = c.String(),
                    })
                .PrimaryKey(t => t.IdTipoProceso);
            
            CreateTable(
                "dbo.UnidadMedida",
                c => new
                    {
                        IdUnidadMedida = c.Int(nullable: false, identity: true),
                        NombreUnidadMedida = c.String(),
                        SiglaUnidad = c.String(),
                    })
                .PrimaryKey(t => t.IdUnidadMedida);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        NombrePrimer = c.String(),
                        NombreSegundo = c.String(),
                        ApePaterno = c.String(),
                        ApeMaterno = c.String(),
                        Sexo = c.String(),
                        FechaNacimiento = c.DateTime(nullable: false),
                        TipoUsuarioId = c.Int(nullable: false),
                        DepartamentoId = c.Int(),
                        EmpresaId = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departamento", t => t.DepartamentoId)
                .ForeignKey("dbo.Empresa", t => t.EmpresaId, cascadeDelete: true)
                .Index(t => t.DepartamentoId)
                .Index(t => t.EmpresaId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Departamento",
                c => new
                    {
                        IdDepartamento = c.Int(nullable: false, identity: true),
                        NombreDepartamento = c.String(),
                    })
                .PrimaryKey(t => t.IdDepartamento);
            
            CreateTable(
                "dbo.Empresa",
                c => new
                    {
                        IdEmpresa = c.Int(nullable: false, identity: true),
                        NombreEmpresa = c.String(),
                        RutEmpresa = c.Int(nullable: false),
                        DgtVerificadorEmpresa = c.Int(nullable: false),
                        EstadoEmpresa = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdEmpresa);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Embalaje",
                c => new
                    {
                        idEmbalaje = c.Int(nullable: false, identity: true),
                        Base = c.Int(nullable: false),
                        AltoCaja = c.Int(nullable: false),
                        CajaEstandar = c.Int(nullable: false),
                        EncajadoxCaja = c.Double(nullable: false),
                        Enzunchado = c.Double(nullable: false),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.idEmbalaje);
            
            CreateTable(
                "dbo.Modulo",
                c => new
                    {
                        IdModulo = c.Int(nullable: false, identity: true),
                        IdSeccion = c.Int(nullable: false),
                        NombreModulo = c.String(maxLength: 150),
                        ControladorModulo = c.String(),
                        Descripcion = c.String(),
                        Orden = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdModulo);
            
            CreateTable(
                "dbo.Presupuesto_SubProceso",
                c => new
                    {
                        IdPresupuestoSubProceso = c.Int(nullable: false, identity: true),
                        PresupuestoId = c.Int(nullable: false),
                        SubProcesoId = c.Int(nullable: false),
                        ValorFijoSubProceso = c.Double(nullable: false),
                        ValorVariableSubProceso = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.IdPresupuestoSubProceso)
                .ForeignKey("dbo.Presupuesto", t => t.PresupuestoId, cascadeDelete: false)
                .ForeignKey("dbo.SubProceso", t => t.SubProcesoId, cascadeDelete: false)
                .Index(t => t.PresupuestoId)
                .Index(t => t.SubProcesoId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Presupuesto_SubProceso", "SubProcesoId", "dbo.SubProceso");
            DropForeignKey("dbo.Presupuesto_SubProceso", "PresupuestoId", "dbo.Presupuesto");
            DropForeignKey("dbo.Presupuesto", "Usuarioid", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "EmpresaId", "dbo.Empresa");
            DropForeignKey("dbo.AspNetUsers", "DepartamentoId", "dbo.Departamento");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Presupuesto", "TapaId", "dbo.Tapa");
            DropForeignKey("dbo.Presupuesto", "MonedaId", "dbo.Moneda");
            DropForeignKey("dbo.Presupuesto", "InteriorId", "dbo.Interior");
            DropForeignKey("dbo.Presupuesto", "FormatoId", "dbo.Formato");
            DropForeignKey("dbo.Presupuesto", "EncuadernacionId", "dbo.Encuadernacion");
            DropForeignKey("dbo.Encuadernacion", "TipoMonedaId", "dbo.TipoMoneda");
            DropForeignKey("dbo.SubProceso", "UnidadMedidaId", "dbo.UnidadMedida");
            DropForeignKey("dbo.SubProceso", "TipoMonedaId", "dbo.TipoMoneda");
            DropForeignKey("dbo.SubProceso", "ProcesoId", "dbo.Proceso");
            DropForeignKey("dbo.Proceso", "TipoProcesoId", "dbo.TipoProceso");
            DropForeignKey("dbo.SubProceso", "DoblezId", "dbo.Doblez");
            DropForeignKey("dbo.Moneda", "TipoMonedaId", "dbo.TipoMoneda");
            DropForeignKey("dbo.Impresion", "TipoMonedaId", "dbo.TipoMoneda");
            DropForeignKey("dbo.Impresion", "MaquinaId", "dbo.Maquina");
            DropForeignKey("dbo.Tapa", "PapelId", "dbo.Papel");
            DropForeignKey("dbo.Papel", "MaquinaId", "dbo.Maquina");
            DropForeignKey("dbo.Interior", "PapelId", "dbo.Papel");
            DropForeignKey("dbo.Formato", "DoblezId", "dbo.Doblez");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Presupuesto_SubProceso", new[] { "SubProcesoId" });
            DropIndex("dbo.Presupuesto_SubProceso", new[] { "PresupuestoId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "EmpresaId" });
            DropIndex("dbo.AspNetUsers", new[] { "DepartamentoId" });
            DropIndex("dbo.Proceso", new[] { "TipoProcesoId" });
            DropIndex("dbo.SubProceso", new[] { "TipoMonedaId" });
            DropIndex("dbo.SubProceso", new[] { "UnidadMedidaId" });
            DropIndex("dbo.SubProceso", new[] { "DoblezId" });
            DropIndex("dbo.SubProceso", new[] { "ProcesoId" });
            DropIndex("dbo.Moneda", new[] { "TipoMonedaId" });
            DropIndex("dbo.Tapa", new[] { "PapelId" });
            DropIndex("dbo.Interior", new[] { "PapelId" });
            DropIndex("dbo.Papel", new[] { "MaquinaId" });
            DropIndex("dbo.Impresion", new[] { "TipoMonedaId" });
            DropIndex("dbo.Impresion", new[] { "MaquinaId" });
            DropIndex("dbo.Encuadernacion", new[] { "TipoMonedaId" });
            DropIndex("dbo.Presupuesto", new[] { "Usuarioid" });
            DropIndex("dbo.Presupuesto", new[] { "MonedaId" });
            DropIndex("dbo.Presupuesto", new[] { "TapaId" });
            DropIndex("dbo.Presupuesto", new[] { "InteriorId" });
            DropIndex("dbo.Presupuesto", new[] { "EncuadernacionId" });
            DropIndex("dbo.Presupuesto", new[] { "FormatoId" });
            DropIndex("dbo.Formato", new[] { "DoblezId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Presupuesto_SubProceso");
            DropTable("dbo.Modulo");
            DropTable("dbo.Embalaje");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Empresa");
            DropTable("dbo.Departamento");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.UnidadMedida");
            DropTable("dbo.TipoProceso");
            DropTable("dbo.Proceso");
            DropTable("dbo.SubProceso");
            DropTable("dbo.Moneda");
            DropTable("dbo.Tapa");
            DropTable("dbo.Interior");
            DropTable("dbo.Papel");
            DropTable("dbo.Maquina");
            DropTable("dbo.Impresion");
            DropTable("dbo.TipoMoneda");
            DropTable("dbo.Encuadernacion");
            DropTable("dbo.Presupuesto");
            DropTable("dbo.Formato");
            DropTable("dbo.Doblez");
        }
    }
}

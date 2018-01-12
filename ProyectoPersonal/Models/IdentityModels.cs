using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ProyectoPersonal.Models.Gerencia;
using ProyectoPersonal.Models.Cotizador;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProyectoPersonal.Models
{
    // Puede agregar datos del perfil del usuario agregando más propiedades a la clase ApplicationUser. Para más información, visite http://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public string NombrePrimer { get; set; }
        public string NombreSegundo { get; set; }
        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string Sexo { get; set; }
        public System.DateTime FechaNacimiento { get; set; }
        public int TipoUsuarioId { get; set; }
        public int? DepartamentoId { get; set; }
        public Departamento Departamento { get; set; }
        public int? EmpresaId { get; set; }
        public Empresa Empresa { get; set; }

        public List<Presupuesto> Presupuestos { get; set; }
        [NotMapped]
        [ScaffoldColumn(false)]
        public string NombreCompleto { get { return NombrePrimer + " " + ApePaterno; } }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Tenga en cuenta que el valor de authenticationType debe coincidir con el definido en CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Agregar aquí notificaciones personalizadas de usuario
            userIdentity.AddClaim(new Claim("NombreCompleto", this.NombrePrimer + " " + this.ApePaterno));
            return userIdentity;
        }
    }

    //public class AplicationRole : IdentityRole
    //{
    //    public AplicationRole() : base() { }
       
    //    public string Descripcion { get; set; }
    //    public string Controller { get; set; }
    //    public string Accion { get; set; }
    //    public int Orden { get; set; }
    //    public bool Menu { get; set; }
    //    public bool Estado { get; set; }
    //    public int Discriminador { get; set; }
    //}
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("IntranetConnectionString", throwIfV1Schema: false)
        {
        }
        
        
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Modulo> MenuPrincipal { get; set; }
        public DbSet<Presupuesto> Presupuesto { get; set; }
        public DbSet<Interior> Interior { get; set; }
        public DbSet<Tapa> Tapa { get; set; }
        public DbSet<Encuadernacion> Encuadernacion { get; set; }
        public DbSet<Formato> Formato { get; set; }
        public DbSet<Maquina> Maquina { get; set; }
        public DbSet<Papel> Papel { get; set; }
        public DbSet<Doblez> Doblez { get; set; }
        public DbSet<TipoMoneda> TipoMoneda { get; set; }
        public DbSet<Moneda> Moneda { get; set; }
        public DbSet<Impresion> Impresion { get; set; }
        public DbSet<UnidadMedida> UnidadMedida { get; set; }
        public DbSet<TipoProceso> TipoProceso { get; set; }
        public DbSet<Proceso> Proceso { get; set; }
        public DbSet<SubProceso> SubProceso { get; set; }
        public DbSet<Presupuesto_SubProceso> Presupuesto_SubProceso { get; set; }
        public DbSet<Embalaje> Embalaje { get; set; }
        public DbSet<TipoCatalogo> Catalogo { get; set; }
        public DbSet<Produccion> Produccion { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().
                HasRequired(x => x.Empresa).WithMany(y=> y.Usuarios)
                .HasForeignKey(x =>x.EmpresaId);
            
            modelBuilder.Entity<ApplicationUser>().
                HasOptional(x => x.Departamento).WithMany(y => y.Usuarios)
                .HasForeignKey(x => x.DepartamentoId);
            
            modelBuilder.Entity<Interior>().
                HasOptional(x => x.Maquina).WithMany(y => y.Interiores)
                .HasForeignKey(x => x.MaquinaId);

            modelBuilder.Entity<Tapa>().
                HasOptional(x => x.Maquina).WithMany(y => y.Tapas)
                .HasForeignKey(x => x.MaquinaId);
            
            modelBuilder.Entity<Formato>().
                HasOptional(x => x.Doblez).WithMany(y => y.Formatos)
                .HasForeignKey(x => x.DoblezId);

            modelBuilder.Entity<Presupuesto>().
                HasOptional(x => x.Formato).WithMany(y => y.Presupuestos)
                .HasForeignKey(x => x.FormatoId);

            modelBuilder.Entity<Presupuesto>().
                HasRequired(x => x.Interior).WithMany(y => y.Presupuestos)
                .HasForeignKey(x => x.InteriorId);

            modelBuilder.Entity<Presupuesto>().
                HasOptional(x => x.Tapa).WithMany(y => y.Presupuestos)
                .HasForeignKey(x => x.TapaId);
            
            modelBuilder.Entity<Presupuesto>().
                HasOptional(x => x.Encuadernacion).WithMany(y => y.Presupuestos)
                .HasForeignKey(x => x.EncuadernacionId);

            modelBuilder.Entity<Presupuesto>().
                HasRequired(x => x.Usuario).WithMany(y => y.Presupuestos)
                .HasForeignKey(x => x.Usuarioid);

            modelBuilder.Entity<Moneda>().
                HasRequired(x => x.TipoMoneda).WithMany(y => y.Monedas)
                .HasForeignKey(x => x.TipoMonedaId);

            modelBuilder.Entity<Encuadernacion>().
                HasOptional(x => x.TipoMoneda).WithMany(y => y.Encuadernaciones)
                .HasForeignKey(x => x.TipoMonedaId);

            modelBuilder.Entity<Impresion>().
                HasOptional(x => x.Maquina).WithMany(y => y.Impresiones)
                .HasForeignKey(x => x.MaquinaId);

            modelBuilder.Entity<Impresion>().
                HasOptional(x => x.TipoMoneda).WithMany(y => y.Impresiones)
                .HasForeignKey(x => x.TipoMonedaId);

            modelBuilder.Entity<Proceso>().
                HasOptional(x => x.TipoProceso).WithMany(y => y.Procesos)
                .HasForeignKey(x => x.TipoProcesoId);

            modelBuilder.Entity<SubProceso>().
                HasRequired(x => x.Proceso).WithMany(y => y.SubProcesos)
                .HasForeignKey(x => x.ProcesoId);

            modelBuilder.Entity<SubProceso>().
                HasRequired(x => x.UnidadMedida).WithMany(y => y.SubProcesos)
                .HasForeignKey(x => x.UnidadMedidaId);

            modelBuilder.Entity<SubProceso>().
                HasRequired(x => x.TipoMoneda).WithMany(y => y.SubProcesos)
                .HasForeignKey(x => x.TipoMonedaId);

            modelBuilder.Entity<SubProceso>().
                HasOptional(x => x.Doblez).WithMany(y => y.SubProcesos)
                .HasForeignKey(x => x.DoblezId);

            //modelBuilder.Entity<SubProceso>().
            //    HasOptional(x => x.Presupuesto_SubProceso).WithMany(y => y.SubProcesos)
            //    .HasForeignKey(x => x.Presupuesto_SubProcesoId);

            //modelBuilder.Entity<Presupuesto>().
            //    HasOptional(x => x.Presupuesto_SubProceso).WithMany(y => y.Presupuestos)
            //    .HasForeignKey(x => x.Presupuesto_ProcesoId);

            //modelBuilder.Entity<Presupuesto_SubProceso>().
            //    HasRequired(x => SubProceso).WithMany()
            //    .HasForeignKey(x => x.IdSubProcesos);

            //modelBuilder.Entity<Presupuesto_SubProceso>().
            //    HasRequired(x => Presupuesto).WithMany()
            //    .HasForeignKey(x => x.IdPresupuesto);


            modelBuilder.Entity<Interior>().
                HasRequired(x => x.Papel).WithMany(y => y.Interiores)
                .HasForeignKey(x => x.PapelId);

            modelBuilder.Entity<Tapa>().
                HasRequired(x => x.Papel).WithMany(y => y.Tapas)
                .HasForeignKey(x => x.PapelId);

            modelBuilder.Entity<Presupuesto>().
                HasRequired(x => x.Moneda).WithMany(y => y.Presupuestos)
                .HasForeignKey(x => x.MonedaId);

            modelBuilder.Entity<Presupuesto>().
                HasRequired(x => x.TipoCatalogo).WithMany(y => y.Presupuestos)
                .HasForeignKey(x => x.TipoCatalogoId);

            modelBuilder.Entity<Papel>().
                HasRequired(x => x.Empresa).WithMany(y => y.Papeles)
                .HasForeignKey(x => x.EmpresaId);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
        

        //public System.Data.Entity.DbSet<ProyectoPersonal.Models.AplicationRole> IdentityRoles { get; set; }
    }
}
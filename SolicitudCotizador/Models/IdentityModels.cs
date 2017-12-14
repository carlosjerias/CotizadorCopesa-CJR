using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SolicitudCotizador.Models
{
    // Puede agregar datos del perfil del usuario agregando más propiedades a la clase ApplicationUser. Para más información, visite http://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Tenga en cuenta que el valor de authenticationType debe coincidir con el definido en CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Agregar aquí notificaciones personalizadas de usuario
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Solicitud> Solicitud { get; set; }
        public DbSet<Proceso> Proceso { get; set; }
        public DbSet<Tiraje> Tiraje { get; set; }
        public DbSet<Formato> Formato { get; set; }
        public DbSet<Encuadernacion> Encuadernacion { get; set; }
        public DbSet<Colores> Colores { get; set; }
        public DbSet<Despacho> Despacho { get; set; }

        public static ApplicationDbContext Create()
        {

            return new ApplicationDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Proceso>().
                HasOptional(x => x.Solicitud).WithMany(y => y.Procesos)
                .HasForeignKey(x => x.SolicitudId);

            modelBuilder.Entity<Solicitud>().
                HasRequired(x => x.Encuadernacion).WithMany(y => y.Solicitudes)
                .HasForeignKey(x => x.EncuadernacionId);

            modelBuilder.Entity<Solicitud>().
                HasRequired(x => x.Formato).WithMany(y => y.Solicitudes)
                .HasForeignKey(x => x.FormatoId);

            modelBuilder.Entity<Solicitud>().
                HasRequired(x => x.Despacho).WithMany(y => y.Solicitudes)
                .HasForeignKey(x => x.DespachoId);

            modelBuilder.Entity<Tiraje>().
                HasRequired(x => x.Solicitud).WithMany(y => y.Tirajes)
                .HasForeignKey(x => x.SolicitudId);

            modelBuilder.Entity<Solicitud>().
                HasRequired(x => x.Cliente).WithMany(y => y.Solicitudes)
                .HasForeignKey(x => x.ClienteId);

            modelBuilder.Entity<Colores>().
                HasOptional(x => x.Proceso).WithMany(y => y.Coloress)
                .HasForeignKey(x => x.ProcesoId);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SolicitudCotizador.Startup))]
namespace SolicitudCotizador
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

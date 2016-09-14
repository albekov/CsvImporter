using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CsvImport.Web.Startup))]
namespace CsvImport.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}

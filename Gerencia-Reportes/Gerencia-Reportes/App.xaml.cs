using System.Globalization;
using System.Threading;
using System.Windows;

namespace Gerencia_Reportes
{
    
    public partial class App : Application
    {


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Establece la cultura a español (España)
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
        }
    }
}

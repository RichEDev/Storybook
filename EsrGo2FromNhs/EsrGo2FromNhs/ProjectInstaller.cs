using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace EsrGo2FromNhs
{
    using System.Diagnostics;

    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            var process = new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem };
            var service = new ServiceInstaller { ServiceName = "EsrGo2FromNhsService" };
            var esrGo2EventLogInstaller = new EventLogInstaller { Source = "Esr Go2 From NHS Service", Log = "ESR Services" };
            Installers.Add(esrGo2EventLogInstaller);
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}


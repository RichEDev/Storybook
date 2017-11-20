using System.ComponentModel;
using System.Configuration.Install;

namespace EsrGo2FromNhsService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}

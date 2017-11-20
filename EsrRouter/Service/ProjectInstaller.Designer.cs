namespace EsrRouter
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.EsrRouterServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.EsrRouterServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.EsrRouterServiceEventLogInstaller = new System.Diagnostics.EventLogInstaller();
            // 
            // EsrRouterServiceProcessInstaller
            // 
            this.EsrRouterServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            this.EsrRouterServiceProcessInstaller.Password = null;
            this.EsrRouterServiceProcessInstaller.Username = null;
            // 
            // EsrRouterServiceInstaller
            // 
            this.EsrRouterServiceInstaller.DelayedAutoStart = true;
            this.EsrRouterServiceInstaller.Description = "This service routes WCF requests between the N3 ESR connector and the SEL file processing web services.";
            this.EsrRouterServiceInstaller.DisplayName = "_File Transfer Service - ESR Router";
            this.EsrRouterServiceInstaller.ServiceName = "_EsrRouter";
            this.EsrRouterServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // EsrRouterServiceEventLogInstaller
            // 
            this.EsrRouterServiceEventLogInstaller.CategoryCount = 0;
            this.EsrRouterServiceEventLogInstaller.CategoryResourceFile = null;
            this.EsrRouterServiceEventLogInstaller.Log = "ESR Services";
            this.EsrRouterServiceEventLogInstaller.MessageResourceFile = null;
            this.EsrRouterServiceEventLogInstaller.ParameterResourceFile = null;
            this.EsrRouterServiceEventLogInstaller.Source = "File Transfer Service - ESR Router";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.EsrRouterServiceEventLogInstaller,
            this.EsrRouterServiceProcessInstaller,
            this.EsrRouterServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller EsrRouterServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller EsrRouterServiceInstaller;
        private System.Diagnostics.EventLogInstaller EsrRouterServiceEventLogInstaller;
    }
}
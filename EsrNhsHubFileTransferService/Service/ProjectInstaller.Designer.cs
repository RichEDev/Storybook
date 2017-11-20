namespace EsrNhsHubFileTransferService
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
            this.EsrNhsHubServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.EsrNhsHubServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.EsrNhsHubServiceEventLogInstaller = new System.Diagnostics.EventLogInstaller();
            // 
            // EsrNhsHubServiceProcessInstaller
            // 
            this.EsrNhsHubServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            this.EsrNhsHubServiceProcessInstaller.Password = null;
            this.EsrNhsHubServiceProcessInstaller.Username = null;
            // 
            // EsrNhsHubServiceInstaller
            // 
            this.EsrNhsHubServiceInstaller.DelayedAutoStart = true;
            this.EsrNhsHubServiceInstaller.Description = "This service monitors ESR for any (version 2) ESR Outbound files.";
            this.EsrNhsHubServiceInstaller.DisplayName = "_File Transfer Service - ESR NHS Hub";
            this.EsrNhsHubServiceInstaller.ServiceName = "_EsrNhsHubFileTransferService";
            this.EsrNhsHubServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // EsrNhsHubServiceEventLogInstaller
            // 
            this.EsrNhsHubServiceEventLogInstaller.CategoryCount = 0;
            this.EsrNhsHubServiceEventLogInstaller.CategoryResourceFile = null;
            this.EsrNhsHubServiceEventLogInstaller.Log = "ESR Services";
            this.EsrNhsHubServiceEventLogInstaller.MessageResourceFile = null;
            this.EsrNhsHubServiceEventLogInstaller.ParameterResourceFile = null;
            this.EsrNhsHubServiceEventLogInstaller.Source = "File Transfer Service - ESR NHS Hub";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.EsrNhsHubServiceProcessInstaller,
            this.EsrNhsHubServiceInstaller,
            this.EsrNhsHubServiceEventLogInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller EsrNhsHubServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller EsrNhsHubServiceInstaller;
        private System.Diagnostics.EventLogInstaller EsrNhsHubServiceEventLogInstaller;
    }
}
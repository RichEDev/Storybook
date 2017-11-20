namespace EsrGo2FromNhs
{
    using System;
    using System.ServiceModel;
    using System.ServiceProcess;

    public partial class EsrGo2FromNhsService : ServiceBase
    {
        public ServiceHost ServiceHost = null;

        public EsrGo2FromNhsService()
        {
            this.InitializeComponent();
            ServiceName = "EsrGo2FromNhsService";
        }

        public static void Main()
        {
            if (Environment.UserInteractive)
            {
                var service = new EsrGo2FromNhsService();
                service.OnStart(new string[] { });
                service.OnStop();
            }
            else
            {
                Run(new EsrGo2FromNhsService());
            }
        }
        
        protected override void OnStart(string[] args)
        {
            if (ServiceHost != null)
            {
                ServiceHost.Close();
            }

            ServiceHost = new ServiceHost(typeof(EsrFileProcessor));
            ServiceHost.Open();
        }

        protected override void OnStop()
        {
            if (this.ServiceHost == null)
            {
                return;
            }

            this.ServiceHost.Close();
            this.ServiceHost = null;
        }
    }
}

using System.ServiceProcess;
using EsrGo2FromNhsWcfLibrary;

namespace EsrGo2FromNhsService
{
    using System;
    using System.ServiceModel;

    public partial class EsrGo2FromNhsService : ServiceBase
    {
        internal static ServiceHost EsrGo2FromNhsServiceHost = null; 

        public EsrGo2FromNhsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ServiceLogger.ImportantInformation("Service starting");

            if (EsrGo2FromNhsServiceHost != null)
            {
                try
                {
                    EsrGo2FromNhsServiceHost.Close();
                }
                catch (Exception exception)
                {
                    ServiceLogger.ImportantError("Error whilst closing the servicehost - {0}", exception.Message);
                }
                
            }
            EsrGo2FromNhsServiceHost = new ServiceHost(typeof(EsrFileProcessor));
            try
            {
                EsrGo2FromNhsServiceHost.Open();
            }
            catch (Exception exception)
            {
                ServiceLogger.ImportantError("Error whilst creating/opening commands servicehost - {0}", exception.Message);
            }
        }

        protected override void OnStop()
        {
            ServiceLogger.ImportantInformation("Service stopping");

            if (EsrGo2FromNhsServiceHost != null)
            {
                try
                {
                    EsrGo2FromNhsServiceHost.Close();
                }
                catch (Exception exception)
                {
                    ServiceLogger.ImportantError("Error whilst closing the servicehost - {0}", exception.Message);
                }

                EsrGo2FromNhsServiceHost.Close();
                EsrGo2FromNhsServiceHost = null;
            }
        }
    }
}

namespace EsrRouter
{
    using System.ServiceProcess;

    using EsrRouter.Service;

    internal static class Program
    {
        #region Methods

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            var servicesToRun = new ServiceBase[] { new EsrRouterWindowsService() };
            ServiceBase.Run(servicesToRun);
        }

        #endregion
    }
}
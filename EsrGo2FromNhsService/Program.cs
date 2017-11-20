using System.ServiceProcess;

namespace EsrGo2FromNhsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] servicesToRun = { 
                                              new EsrGo2FromNhsService() 
                                          };
            ServiceBase.Run(servicesToRun);
        }
    }
}

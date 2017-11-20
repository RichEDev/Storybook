namespace ScheduledTaskHandler
{
    using BusinessLogic.Accounts;
    using BusinessLogic.DataConnections;

    using Common.Logging;

    using Bootstrap;
    using Core;
    using ProjectCodes;

    using System;
    using System.Collections.Generic;

    using CommandLine;

    using SimpleInjector;


    class Program
    {
        /// <summary>
        /// Sets the context of the dependency injection framework to use the specified <paramref name="accountId"/>.
        /// </summary>
        /// <param name="accountId">The account Id to set the current context for.</param>
        /// <returns>An instance of <see cref="Container"/> to enable the retrieval of dependencies.</returns>
        public static Container SetContext(int? accountId)
        {
            Container container = Bootstrapper.Bootstrap(accountId);

            IExtraContext extraContext = container.GetInstance<IExtraContext>();

            if (accountId.HasValue)
            {

                extraContext["accountid"] = accountId;
            }
            else
            {
                extraContext.Remove("accountid");    
            }

            return container;
        }

        static void Main(string[] args)
        {
            // Set the default context so we can get accounts etc but not under the context of a specific account 
            Container container = SetContext(null);
            
            // Create a loader and populate with all available accounts (including archived etc).
            IDataFactory<IAccount, int> accounts = container.GetInstance<IDataFactory<IAccount, int>>();
            IList<IAccount> accountCollection = accounts.Get();
            JobManager manager = new JobManager(new Looper(accountCollection), new LogFactory<JobManager>().GetLogger());


            var options = new Options();
            var isValid = Parser.Default.ParseArgumentsStrict(args, options);

            if (isValid)
            {
                // Load any jobs based on command line options.
                Loader(manager, options);
                
                // Execute any jobs that have been created.
                manager.StartJobs();
            }

            if (isValid || manager.Count == 0)
            {
                Console.WriteLine(options.GetUsage());
            }

            // End the application with a success state
            Environment.Exit(0);
        }

        /// <summary>
        /// Allows the loading of <see cref="Job"/> objects into the <see cref="JobManager"/> whilst checking <see cref="Options"/> to see what command line arguments were passed.
        /// </summary>
        /// <param name="manager">The <see cref="JobManager"/> to add <see cref="Job"/> instances to.</param>
        /// <param name="options">The command line options as an instance of <see cref="Options"/>.</param>
        static void Loader(JobManager manager, Options options)
        {
            // Add jobs to the manager.
            if (options.ProjectCodesRedisPreLoaderJob)
            {
                manager.Add(new ProjectCodesRedisPreLoaderJob(new LogFactory<ProjectCodesRedisPreLoaderJob>().GetLogger()));
            }
        }
    }
}

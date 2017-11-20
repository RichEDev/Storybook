namespace ScheduledTaskHandler.ProjectCodes
{
    using BusinessLogic.Accounts;
    using BusinessLogic.DataConnections;
    using BusinessLogic.ProjectCodes;
    using SimpleInjector;

    using Core;
    
    using Common.Logging;

    /// <summary>
    /// Pre-loads all project codes for an account into Redis.
    /// </summary>
    internal class ProjectCodesRedisPreLoaderJob : Job
    {
        /// <inheritdoc />
        public ProjectCodesRedisPreLoaderJob(ILog logger) : base(logger)
        {
        }

        /// <summary>
        /// Pre-loads all project codes for an account into Redis.
        /// </summary>
        /// <param name="account">The <see cref="IAccount"/> to exceute this method on.</param>
        /// <param name="container">An instance of <see cref="Container"/> to obtain instances of objects from.</param>
        public override void Action(IAccount account, Container container)
        {
            IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int> projectCodes = container.GetInstance<IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int>>();

            this.Log.Info($"Caching {projectCodes.Get().Count} Project Codes");
        }

        /// <inheritdoc />
        public override void Execute(ILooper looper)
        {
            looper.Iterate(this.Action, account => account.Archived == false);
        }
    }
}

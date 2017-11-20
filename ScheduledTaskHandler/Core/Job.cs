namespace ScheduledTaskHandler.Core
{
    using BusinessLogic.Accounts;

    using Common.Logging;

    using SimpleInjector;

    using System;

    /// <summary>
    /// Defines the required memembers for a <see cref="Job"/> to be executed.
    /// </summary>
    public abstract class Job
    {
        /// <summary>
        /// An instance of <see cref="ILog"/> to enable logging throughout the <see cref="Job"/>.
        /// </summary>
        public ILog Log { get; }

        /// <summary>
        /// The name of this job.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="Job"/> class. 
        /// </summary>
        /// <param name="logger">An instance of <see cref="ILog"/> to enable logging throughout the <see cref="Job"/>.</param>
        protected Job(ILog logger)
        {
            this.Log = logger;
            this.Type = this.GetType();

            if (this.Log.IsInfoEnabled)
            {
                this.Log.Info($"{this.Type.Name} job registered for execution.");
            }
        }

        /// <summary>
        /// Executes before this <see cref="Job"/> <code>Execute</code> has been called to enable any inital logging.
        /// </summary>
        public void PreExecute()
        {
            if (this.Log.IsDebugEnabled)
            {
                this.Log.Debug($"Starting job {this.Type.Name}.");
            }
        }

        /// <summary>
        /// Executes after this <see cref="Job"/> <code>Execute</code> has been called to enable any final logging.
        /// </summary>
        public void PostExecute()
        {
            if (this.Log.IsDebugEnabled)
            {
                this.Log.Debug($"Finished job {this.Type.Name}.");
            }
        }

        /// <summary>
        /// A method which should be executed <paramref name="account"/>.
        /// </summary>
        /// <param name="account">The <see cref="IAccount"/> to exceute this method on.</param>
        /// <param name="container">An instance of <see cref="Container"/> to obtain instances of objects from.</param>
        public abstract void Action(IAccount account, Container container);

        /// <summary>
        /// Attaches this <see cref="Job"/> to a an instance of <see cref="ILooper"/> for execution.
        /// </summary>
        /// <param name="looper"></param>
        public abstract void Execute(ILooper looper);
    }
}

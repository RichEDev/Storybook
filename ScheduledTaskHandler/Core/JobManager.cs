namespace ScheduledTaskHandler.Core
{
    using Common.Logging;

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Manages collections of <see cref="Job"/> objects.
    /// </summary>
    internal class JobManager
    {
        /// <summary>
        /// Collection of <see cref="Job"/> objects to manage.
        /// </summary>
        private readonly List<Job> _jobs;

        /// <summary>
        /// An instance of <see cref="ILooper"/> this <see cref="JobManager"/> will use for execution.
        /// </summary>
        private readonly ILooper _looper;

        private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobManager"/> class. 
        /// </summary>
        /// <param name="looper">An instance of <see cref="ILooper"/> this <see cref="JobManager"/> will use for execution.</param>
        /// <param name="log">An instance of <see cref="ILog"/> this <see cref="JobManager"/> will use to log messages.</param>
        public JobManager(ILooper looper, ILog log)
        {
            this._jobs = new List<Job>();
            this._looper = looper;
            this._log = log;
        }

        /// <summary>
        /// Adds a <see cref="Job"/> to be managed by this <see cref="JobManager"/>.
        /// </summary>
        /// <param name="job">The <see cref="Job"/> to manage.</param>
        public void Add(Job job)
        {
            this._jobs.Add(job);
        }

        /// <summary>
        /// Gets the number of <see cref="Job"/> elements created.
        /// </summary>
        public int Count => this._jobs.Count;

        /// <summary>
        /// Starts executing each <see cref="Job"/> associated to this <see cref="JobManager"/>.
        /// </summary>
        public void StartJobs()
        {
            foreach (Job job in this._jobs)
            {
                try
                {
                    job.PreExecute();
                    job.Execute(this._looper);
                    job.PostExecute();
                }
                catch (Exception ex)
                {
                    // Generic exception handler so no job can cause unhandled exceptions
                    if (this._log.IsErrorEnabled)
                    {
                        this._log.Error($"{job.Type.Name} failed to complete.", ex);
                    }
                }
            }
        }
    }
}

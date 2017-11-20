namespace ScheduledTaskHandler
{
    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// Options class for parsing command line arguments.
    /// </summary>
    class Options
    {
        // See for documentation: https://github.com/gsscoder/commandline
        /// <summary>
        /// Gets a value indicating whether or not this job should be executed.
        /// </summary>
        [Option(HelpText = "Whether or not to execute the loading of project codes into cache.")]
        public bool ProjectCodesRedisPreLoaderJob { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("ScheduledTaskHandler"),
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("Usage: app --ProjectCodesRedisPreLoaderJob");
            help.AddOptions(this);
            return help;
        }
    }
}

namespace InternalApi.Bootstrap
{
    using Configuration.Interface;

    using SEL.FeatureFlags.Configuration;

    using SimpleInjector;

    /// <summary>
    /// Bootstrapper for obtaining the <see cref="IFeatureFlagConfiguration"/>.
    /// </summary>
    internal class BootstrapFileSystemConfiguration
    {
        /// <summary>
        /// Creates an instance of <see cref="IFeatureFlagConfiguration"/>.
        /// </summary>
        /// <param name="container">An instance of the DI container.</param>
        /// <returns>an instance of <see cref="IFeatureFlagConfiguration"/>.</returns>
        public static IFeatureFlagConfiguration CreateNew(Container container)
        {
            IConfigurationManager configurationManager = container.GetInstance<IConfigurationManager>();
            IFeatureFlagConfiguration featureFlagConfiguration = new FileSystemConfiguration(configurationManager.AppSettings["FeatureFlagConfigPath"], container.GetInstance<IFileSystem>(), container.GetInstance<IFileWatcher>());

            return featureFlagConfiguration;
        }
    }
}
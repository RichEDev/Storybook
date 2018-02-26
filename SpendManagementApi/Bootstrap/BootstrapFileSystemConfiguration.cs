namespace SpendManagementApi.Bootstrap
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
        /// Creates an instance of <see cref="IAccount"/> for the <see cref="UserIdentity"/> and this request.
        /// </summary>
        /// <param name="container">An instance of the DI container to get the current user from.</param>
        /// <returns>an instance of <see cref="IAccount"/> for the <see cref="UserIdentity"/> and this request or null if the <see cref="UserIdentity"/> is in an invalid state.</returns>
        public static IFeatureFlagConfiguration CreateNew(Container container)
        {
            IConfigurationManager configurationManager = container.GetInstance<IConfigurationManager>();
            IFeatureFlagConfiguration featureFlagConfiguration = new FileSystemConfiguration(configurationManager.AppSettings["FeatureFlagConfigPath"], container.GetInstance<IFileSystem>(), container.GetInstance<IFileWatcher>());

            return featureFlagConfiguration;
        }
    }
}
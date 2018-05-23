namespace InternalApi.Bootstrap
{
    using System;
    using System.Web.Http;

    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Announcements;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Employees.AccessRoles;
    using BusinessLogic.Fields;
    using BusinessLogic.Identity;
    using BusinessLogic.Images;
    using BusinessLogic.Tables;
    using BusinessLogic.UserDefinedFields;

    using CacheDataAccess.Caching;

    using global::Common.Logging;
    using global::Common.Logging.Log4Net;

    using Configuration.Core;
    using Configuration.Interface;    

    using InternalApi.Bootstrap;

    using SEL.FeatureFlags;
    using SEL.FeatureFlags.Configuration;
    using SEL.FeatureFlags.Context;
    using SEL.MessageBrokers;
    using SEL.MessageBrokers.RabbitMQ;

    using SimpleInjector;
    using SimpleInjector.Lifestyles;

    using SQLDataAccess;
    using SQLDataAccess.Accounts;
    using SQLDataAccess.Employees.AccessRoles;
    using SQLDataAccess.ProjectCodes;
    using SQLDataAccess.Tables;
    using SQLDataAccess.UserDefinedFieldValues;

    using Utilities.Cryptography;

    /// <summary>
    /// Bootstrap the applications dependency injection
    /// </summary>
    public class Bootstrapper
    {
        /// <summary>
        /// Bootstrap the applications dependency injection
        /// </summary>
        /// <returns>A populated instance of <see cref="Container"/>.</returns>
        internal static Container Bootstrap()
        {
            Container container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // Register your types, for instance using the scoped lifestyle:
            container.Register<IConfigurationManager, WebConfigurationManagerAdapter>(Lifestyle.Singleton);

            container.Register<ICryptography, ExpensesCryptography>(Lifestyle.Singleton);
            container.Register<IAccount>(() => BootstrapAccount.CreateNew(container), Lifestyle.Transient);

            container.Register<IIdentityContextProvider, WebIdentityContext>();
            container.Register<IIdentityProvider, IdentityProvider>();

            // Logging
            container.Register<IExtraContext, Log4NetContextAdapter>();
            container.RegisterConditional(typeof(ILog), c => typeof(Log4NetAdapter<>).MakeGenericType(c.Consumer.ImplementationType), Lifestyle.Singleton, c => true);

            // Generic registrations.

            container.Register(typeof(ICustomerDataConnection<>), typeof(CustomerDatabaseConnection));
            container.Register(typeof(IMetabaseDataConnection<>), typeof(MetabaseDataConnection));
            container.Register(typeof(DataParameters<>), typeof(SqlDataParameters));
            container.Register(typeof(IMetabaseCacheKey<>), typeof(MetabaseCacheKey<>));
            container.Register(typeof(IAccountCacheKey<>), typeof(AccountCacheKey<>));

            container.Register<ISerialize, BinarySerializer>(Lifestyle.Singleton);
            container.Register(typeof(ICache<,>), typeof(RedisCache<,>), Lifestyle.Singleton);
            container.Register(typeof(RepositoryBase<,>), typeof(RepositoryBase<,>));
            container.Register(typeof(IMetabaseCacheFactory<,>), typeof(MetabaseCacheFactory<,>));
            container.Register(typeof(IAccountCacheFactory<,>), typeof(AccountCacheFactory<,>));
            container.Register(typeof(IDataFactory<IAnnouncement, Guid>), typeof(AnnouncementsCacheFactory));
            container.Register(typeof(IRpcClient), typeof(RabbitMqRpcClient));
            container.Register(typeof(IImageConversion), typeof(JpgImageConversion));
            container.Register(typeof(IImageManipulation), typeof(JpgImageManipulation));
            container.Register(typeof(ReadReceiptFactory), typeof(ReadReceiptFactory));

            container.Register<UserDefinedFieldValueRepository, SqlUserDefinedFieldValuesFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(UserDefinedFieldValueRepository), new[] { typeof(SqlUserDefinedFieldValuesFactory) });

            container.Register<UserDefinedFieldRepository, SqlUserDefinedFieldFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(UserDefinedFieldRepository), typeof(SqlUserDefinedFieldFactory).Assembly, typeof(UserDefinedFieldRepository).Assembly);
            container.Register<TableRepository, SqlTableFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(TableRepository), typeof(SqlTableFactory).Assembly, typeof(TableRepository).Assembly);
            container.Register<IFieldFactory, FieldFactory>(Lifestyle.Transient);

            container.Register<IAssignedAccessRolesFactory, SqlEmployeeAssignedAccessRoles>(Lifestyle.Transient);
            container.Register<IEmployeeCombinedAccessRoles, SqlEmployeeCombinedAccessRolesFactory>(Lifestyle.Transient);

            // This registration should register all implementations of IDataFactory<,> within the SqlDataAccess project. 
            container.Register(typeof(IDataFactory<,>), new[] { typeof(SqlAccountFactory).Assembly });
            container.Register(typeof(IDataFactoryCustom<,,>), new[] { typeof(SqlProjectCodesWithUserDefinedValuesFactory).Assembly });

            container.Register<IFileWatcher, FileWatcher>();
            container.Register<IFileSystem, FileSystem>();
            container.Register<IFeatureFlagConfiguration>(() => BootstrapFileSystemConfiguration.CreateNew(container));
            container.Register<IFeatureFlagsContextProvider, WebFeatureFlagContext>();
            container.Register<IFeatureFlagManager, FeatureFlagManager>();

            // This is an extension method from the integration package.
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            return container;
        }
    }
}
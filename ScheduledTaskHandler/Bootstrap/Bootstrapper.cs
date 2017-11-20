namespace ScheduledTaskHandler.Bootstrap
{
    using BusinessLogic;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields;
    using BusinessLogic.Identity;
    using BusinessLogic.Tables;
    using BusinessLogic.UserDefinedFields;
    using CacheDataAccess.Caching;
    using Common.Logging;
    using Common.Logging.Log4Net;
    using SimpleInjector;
    using SQLDataAccess;
    using SQLDataAccess.Accounts;
    using SQLDataAccess.ProjectCodes;
    using SQLDataAccess.Tables;
    using SQLDataAccess.UserDefinedFieldValues;
    using Utilities.Cryptography;

    using Configuration.Core;
    using Configuration.Interface;

    /// <summary>
    /// Bootstrap the applications dependency injection
    /// </summary>
    public class Bootstrapper
    {
        public static int AccountId { get; set; }
        /// <summary>
        /// Bootstrap the applications dependency injection
        /// </summary>
        /// <returns>A populated instance of <see cref="Container"/>.</returns>
        internal static Container Bootstrap(int? accountId)
        {
            if (accountId.HasValue)
            {
                AccountId = accountId.Value;
            }

            Container container = new Container();

            // Register your types, for instance using the scoped lifestyle:
            container.Register<IConfigurationManager, ConfigurationManagerAdapter>(Lifestyle.Singleton);

            container.Register<ICryptography, ExpensesCryptography>(Lifestyle.Singleton);
            container.Register(() => BootstrapAccount.CreateNew(container));

            container.Register<IIdentityContextProvider, ConsoleIdentityContext>();
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
            
            container.Register<UserDefinedFieldValueRepository, SqlUserDefinedFieldValuesFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(UserDefinedFieldValueRepository), new[] { typeof(SqlUserDefinedFieldValuesFactory) });

            container.Register<UserDefinedFieldRepository, SqlUserDefinedFieldFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(UserDefinedFieldRepository), typeof(SqlUserDefinedFieldFactory).Assembly, typeof(UserDefinedFieldRepository).Assembly);
            container.Register<TableRepository, SqlTableFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(TableRepository), typeof(SqlTableFactory).Assembly, typeof(TableRepository).Assembly);
            container.Register<IFieldFactory, FieldFactory>(Lifestyle.Transient);

            // This registration should register all implementations of IDataFactory<,> within the SqlDataAccess project. 
            container.Register(typeof(IDataFactory<,>), new[] { typeof(SqlAccountFactory).Assembly });
            container.Register(typeof(IDataFactoryCustom<,>), new[] { typeof(SqlProjectCodesWithUserDefinedValuesFactory).Assembly });

            return container;
        }
    }
}
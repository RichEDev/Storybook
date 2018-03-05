using Common.Logging;
using Common.Logging.Log4Net;

namespace PublicAPI.Bootstrap
{
    using System.Web.Http;

    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.Identity;
    using BusinessLogic.Images;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields;
    using BusinessLogic.Tables;
    using BusinessLogic.UserDefinedFields;

    using CacheDataAccess.Caching;

    using Configuration.Core;
    using Configuration.Interface;

    using SEL.MessageBrokers;
    using SEL.MessageBrokers.RabbitMQ;

    using SimpleInjector;
    using SimpleInjector.Lifestyles;

    using SQLDataAccess;
    using SQLDataAccess.Accounts;
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
            container.Register<IAccount>(() => BootstrapAccount.CreateNew(container), Lifestyle.Scoped);

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
            container.Register(typeof(IRpcClient), typeof(RabbitMqRpcClient));
            container.Register(typeof(IImageConversion), typeof(JpgImageConversion));
            container.Register(typeof(IImageManipulation), typeof(JpgImageManipulation));

            container.Register<UserDefinedFieldValueRepository, SqlUserDefinedFieldValuesFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(UserDefinedFieldValueRepository), new[] { typeof(SqlUserDefinedFieldValuesFactory) });

            container.Register<UserDefinedFieldRepository, SqlUserDefinedFieldFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(UserDefinedFieldRepository), typeof(SqlUserDefinedFieldFactory).Assembly, typeof(UserDefinedFieldRepository).Assembly);
            container.Register<TableRepository, SqlTableFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(TableRepository), typeof(SqlTableFactory).Assembly, typeof(TableRepository).Assembly);
            container.Register<IFieldFactory, FieldFactory>(Lifestyle.Transient);

            //container.Register<IDataFactory<IProjectCode, int>, SqlProjectCodesFactory>();
            //container.Register<IDataFactory<IAccount, int>, SqlAccountFactory>();

            // This registration should register all implementations of IDataFactory<,> within the SqlDataAccess project. 
            container.Register(typeof(IDataFactory<,>), new[] { typeof(SqlAccountFactory).Assembly });
            container.Register(typeof(IDataFactoryCustom<,>), new[] { typeof(SqlProjectCodesWithUserDefinedValuesFactory).Assembly });

            // This is an extension method from the integration package.
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            return container;
        }
    }
}
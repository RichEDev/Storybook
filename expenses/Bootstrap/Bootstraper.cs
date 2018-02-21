namespace expenses.Bootstrap
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Remoting.Channels.Http;
    using System.Web.Compilation;
    using System.Web.UI;

    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.CustomEntities;
    using BusinessLogic.CustomFields;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields;
    using BusinessLogic.Fields.Type.ValueList;
    using BusinessLogic.FinancialYears;
    using BusinessLogic.Identity;
    using BusinessLogic.ProjectCodes;
    using BusinessLogic.Tables;
    using BusinessLogic.UserDefinedFields;

    using CacheDataAccess.Caching;
    using CacheDataAccess.CustomFields;
    using CacheDataAccess.Fields;

    using Common.Cryptography;
    using Common.Logging;
    using Common.Logging.Log4Net;
    using Configuration.Core;
    using Configuration.Interface;

    using SEL.FeatureFlags;
    using SEL.FeatureFlags.Configuration;
    using SEL.FeatureFlags.Context;

    using SimpleInjector;
    using SimpleInjector.Diagnostics;
    using SimpleInjector.Integration.Web;
    using SQLDataAccess;
    using SQLDataAccess.Accounts;
    using SQLDataAccess.CustomEntities;
    using SQLDataAccess.CustomFields;
    using SQLDataAccess.Fields;
    using SQLDataAccess.FinancialYears;
    using SQLDataAccess.ProjectCodes;
    using SQLDataAccess.Tables;
    using SQLDataAccess.UserDefinedFieldValues;

    using Utilities.Cryptography;

    using System.Web;

    /// <summary>
    /// Bootstrapper class for SimpleInjector and Expenses
    /// </summary>
    public static class Bootstraper
    {
        internal static Container Bootstrap()
        {
            // Create a new Simple Injector container.
            var container = new Container();

            // Register a custom PropertySelectionBehavior to enable property injection.
            container.Options.PropertySelectionBehavior = new DependencyAttributePropertySelectionBehavior();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            // Low level classes
            container.Register<IConfigurationManager, WebConfigurationManagerAdapter>(Lifestyle.Singleton);
            
            container.Register<ICryptography, ExpensesCryptography>(Lifestyle.Singleton);

            // Creation of the currently logged in account
            container.Register<IAccount>(() => BootstrapAccount.CreateNew(container), Lifestyle.Scoped);

            container.RegisterConditional<IIdentityContextProvider, SelfRegistrationIdentityContext>(Lifestyle.Scoped, context => HttpContext.Current.Request.Path.ToLower().Contains("register.aspx"));
            container.RegisterConditional<IIdentityContextProvider, WebIdentityContext>(Lifestyle.Scoped, c => !c.Handled);
            container.Register<IIdentityProvider, IdentityProvider>(Lifestyle.Scoped);

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
            container.Register(typeof(IEncryptor), typeof(Pbkdf2Encryptor), Lifestyle.Singleton);

            // This registration should register all implementations of IDataFactory<,> within the SqlDataAccess project. 
            container.Register(typeof(IDataFactory<,>), new[] { typeof(SqlAccountFactory).Assembly });
            container.Register(typeof(IDataFactoryCustom<,>), new[] { typeof(SqlProjectCodesWithUserDefinedValuesFactory).Assembly });
            
            // Register the lazy for places such as self reg
            container.Register(() => new Lazy<IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int>>(container.GetInstance<IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int>>));

            container.Register<FieldRepository, SqlFieldFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(FieldRepository), typeof(SqlFieldFactory).Assembly, typeof(CacheFieldFactory).Assembly);

            container.Register<CustomFieldRepository, CacheCustomFieldsFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(CustomFieldRepository), typeof(SqlCustomFieldFactory).Assembly, typeof(CacheCustomFieldsFactory).Assembly);

            container.Register<CustomEntityFieldRepository, CacheCustomEntityFieldsFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(CustomEntityFieldRepository), typeof(SqlCustomEntityFieldsFactory).Assembly, typeof(CacheCustomEntityFieldsFactory).Assembly);

            container.Register<FieldListValuesRepository, SqlFieldListValueFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(FieldListValuesRepository), new[] { typeof(SqlFieldListValueFactory) });

            container.Register<CustomFieldListValuesRepository, SqlCustomFieldListvalueFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(CustomFieldListValuesRepository), new[] { typeof(SqlCustomFieldListvalueFactory) });

            container.Register<CustomEntityFieldListValuesRepository, SqlCustomEntityFieldListValuesFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(CustomEntityFieldListValuesRepository), new[] { typeof(SqlCustomEntityFieldListValuesFactory) });

            container.Register<UserDefinedFieldValueRepository, SqlUserDefinedFieldValuesFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(UserDefinedFieldValueRepository), new[] { typeof(SqlUserDefinedFieldValuesFactory) });

            container.Register<UserDefinedFieldRepository, SqlUserDefinedFieldFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(UserDefinedFieldRepository), typeof(SqlUserDefinedFieldFactory).Assembly, typeof(UserDefinedFieldRepository).Assembly);

            container.Register<TableRepository, SqlTableFactory>(Lifestyle.Transient);
            container.RegisterCollection(typeof(TableRepository), typeof(SqlTableFactory).Assembly, typeof(TableRepository).Assembly);

            container.Register<IFieldFactory, FieldFactory>(Lifestyle.Transient);

            container.Register<IActionContext, ActionContext>(Lifestyle.Transient);

            container.Register<FinancialYearRepository, SqlFinancialYearFactory>(Lifestyle.Transient);
            container.Register<HttpRemotingHandler>(BootstrapHttpRemotingHandler.New);

            container.Register<IFileWatcher, FileWatcher>(Lifestyle.Singleton);
            container.Register<IFileSystem, FileSystem>(Lifestyle.Singleton);
            container.Register<IFeatureFlagConfiguration>(() => BootstrapFileSystemConfiguration.CreateNew(container), Lifestyle.Scoped);
            container.Register<IFeatureFlagsContextProvider, WebFeatureFlagContext>();
            container.Register<IFeatureFlagManager, FeatureFlagManager>();

            RegisterWebPages(container);

            // Verify verifies that all required dependencies are registered
            // ONLY RUN IN DEBUG MODE
            #if DEBUG
              //container.Verify(); 
            #endif
            
            return container;
        }

        /// <summary>
        /// Registers each <see cref="Page"/> for simple injector usage
        /// </summary>
        /// <param name="container"></param>
        private static void RegisterWebPages(Container container)
        {
            var pageTypes =
                from assembly in BuildManager.GetReferencedAssemblies().Cast<Assembly>()
                where !assembly.IsDynamic
                where !assembly.GlobalAssemblyCache
                from type in assembly.GetExportedTypes()
                where type.IsSubclassOf(typeof(Page)) || type.IsSubclassOf(typeof(System.Web.Services.WebService))
                where !type.IsAbstract && !type.IsGenericType
                select type;

            foreach (Type type in pageTypes)
            {
                var registration = Lifestyle.Transient.CreateRegistration(type, container);
                registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "ASP.NET creates and disposes page classes for us.");
                container.AddRegistration(type, registration);
            }
        }
    }

    /// <summary>
    /// An implimentation of <see cref="HttpRemotingHandler"/> to hide the second constructor from Simple Injector.
    /// </summary>
    public static class BootstrapHttpRemotingHandler
    {
        private static readonly HttpRemotingHandler RemotingHandler;

        static BootstrapHttpRemotingHandler()
        {
            RemotingHandler = new HttpRemotingHandler();
        }

        /// <summary>
        /// Create a new instance of <see cref="HttpRemotingHandler"/>
        /// </summary>
        /// <returns>An instance of <see cref="HttpRemotingHandler"/></returns>
        public static HttpRemotingHandler New()
        {
            return RemotingHandler;
        }
    }
}
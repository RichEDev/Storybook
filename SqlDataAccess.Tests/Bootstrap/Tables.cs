namespace SqlDataAccess.Tests.Bootstrap
{
    using BusinessLogic.Tables;

    using Common.Logging;

    using NSubstitute;

    /// <summary>
    /// The Mocked tables objects for Unit Tests.
    /// </summary>
    public class Tables : ITables
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tables"/> class.
        /// </summary>
        /// <param name="logger">
        /// An instance of <see cref="ILog"/>.
        /// </param>
        public Tables(ILog logger)
        {
            this.TableRepository = Substitute.For<TableRepository>(logger);
        }
        
        /// <summary>
        /// Gets or sets the Mock'd <see cref="ITables.TableRepository"/>
        /// </summary>
        public TableRepository TableRepository { get; set; }

    }
}
namespace SpendManagementApi.Repositories
{
    using SpendManagementApi.Common;
    using SpendManagementApi.Interfaces;

    using Spend_Management;

    /// <summary>
    /// The basic repository class.
    /// </summary>
    public abstract class BasicRepository : IBasicRepository
    {       
        /// <summary>
        /// The _action context.
        /// </summary>
        private IActionContext _actionContext;

        /// <summary>
        /// Gets the user.
        /// </summary>
        public ICurrentUser User { get; }

        /// <summary>
        /// Gets or sets the action context.
        /// </summary>
        public IActionContext ActionContext
        {
            get
            {
                this._actionContext = this._actionContext ?? new ActionContext(User);
                return this._actionContext;
            }

            set
            {
                this._actionContext = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicRepository"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        protected BasicRepository (ICurrentUser user)
        {         
            this.User = user;
        }      
    }
}
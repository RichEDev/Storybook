namespace ScheduledTaskHandler.Core
{
    using System.Collections.Generic;
    using BusinessLogic.Accounts;

    using SimpleInjector;

    using System;

    /// <inheritdoc />
    internal class Looper : ILooper
    {
        /// <summary>
        /// >A <see cref="IList{T}"/> of <see cref="IAccount"/> objects to iterate on.
        /// </summary>
        private readonly IList<IAccount> _accounts;

        /// <summary>
        /// Initializes a new instance of the <see cref="Looper"/> class. 
        /// </summary>
        /// <param name="accounts">A <see cref="IList{T}"/> of <see cref="IAccount"/> objects to iterate on.</param>
        internal Looper(IList<IAccount> accounts)
        {
            this._accounts = accounts;
        }

        /// <inheritdoc />
        public void Iterate(Action<IAccount, Container> method)
        {
            this.Iterate(method, null);
        }

        /// <inheritdoc />
        public void Iterate(Action<IAccount, Container> method, Predicate<IAccount> predicate)
        {
            foreach (IAccount account in this._accounts)
            {
                if (predicate == null || predicate.Invoke(account))
                {
                    Container container = Program.SetContext(account.Id);
                    method(account, container);
                    Program.SetContext(null);
                }
            }
        }
    }
}

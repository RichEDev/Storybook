namespace ScheduledTaskHandler.Core
{
    using BusinessLogic.Accounts;

    using SimpleInjector;

    using System;

    /// <summary>
    /// Defines methods for iterating on instances of <see cref="IAccount"/>.
    /// </summary>
    public interface ILooper
    {
        /// <summary>
        /// Executes <paramref name="method"/> on each available <see cref="IAccount"/>.
        /// </summary>
        /// <param name="method">The method to execute.</param>
        void Iterate(Action<IAccount, Container> method);

        /// <summary>
        /// Executes <paramref name="method"/> on each available <see cref="IAccount"/> that matches <paramref name="predicate"/>.
        /// </summary>
        /// <param name="method">The method to execute.</param>
        /// <param name="predicate">The criteria to match each <see cref="IAccount"/> on.</param>
        void Iterate(Action<IAccount, Container> method, Predicate<IAccount> predicate);
    }
}

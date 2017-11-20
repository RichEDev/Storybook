namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;

    using Interfaces;
    using Models.Types;

    using Spend_Management;
    using SML = SpendManagementLibrary;

    /// <summary>
    /// TooltipRepository manages data access for Tooltips.
    /// </summary>
    internal class TooltipRepository : BaseRepository<Tooltip>, ISupportsActionContext
    {
        /// <summary>
        /// Creates a new TooltipRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public TooltipRepository(ICurrentUser user, IActionContext actionContext) 
            : base(user, actionContext, x => x.Id, x => x.HelpText)
        {
        }

        /// <summary>
        /// Gets a single Tooltip by it's guid identifier.
        /// </summary>
        /// <param name="id">The id of the Tooltip to get.</param>
        /// <returns>The Tooltip.</returns>
        public Tooltip Get(Guid id)
        {
            return new Tooltip(id, this.User.AccountID);
        }

        public override IList<Tooltip> GetAll()
        {
            throw new NotImplementedException();
        }

        public override Tooltip Get(int id)
        {
            throw new NotImplementedException();
        }
    }
}
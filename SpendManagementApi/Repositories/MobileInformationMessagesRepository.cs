namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Repositories;

    using SpendManagementLibrary.MobileInformationMessage;

    using Spend_Management;

    using MobileInformationMessage = SpendManagementApi.Models.Types.InformationMessage.MobileInformationMessage;

    internal class MobileInformationMessagesRepository : BaseRepository<MobileInformationMessage>, ISupportsActionContext
    {
        /// <summary>
        /// The _action context.
        /// </summary>
        private readonly IActionContext _actionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileInformationMessagesRepository"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        public MobileInformationMessagesRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, x => x.InformationId, x => x.Message)
        {
            this._actionContext = actionContext;
        }

        /// <summary>
        /// Gets all the active <see cref="MobileInformationMessage"/>
        /// </summary>
        /// <returns>
        /// The <see cref="IList"/> of <see cref="MobileInformationMessage"/> .
        /// </returns>
        public override IList<MobileInformationMessage> GetAll()
        {
            var mobileInformationMessages = MobileInformationMessages.GetMobileInformationMessages();
            return mobileInformationMessages.Select(x => new MobileInformationMessage().ToApiType(x, this._actionContext)).ToList();
        }
        public override MobileInformationMessage Get(int id)
        {
            throw new NotImplementedException();
        }
    }
}
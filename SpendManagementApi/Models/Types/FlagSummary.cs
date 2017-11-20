namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;

    using SpendManagementLibrary.Flags;

    /// <summary>
    /// A flagged item and associated flag id.
    /// </summary>
    public class FlagSummary : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Flags.FlagSummary, FlagSummary>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FlagSummary"/> class.
        /// </summary>
        /// <param name="flagId">
        /// The flag id.
        /// </param>
        /// <param name="flaggedItem">
        /// The instance of the flagged item.
        /// </param>
        public FlagSummary(int flagId, FlaggedItem flaggedItem)
        {
            this.FlagId = flagId;
            this.FlaggedItem = flaggedItem;
        }

        /// <summary>
        /// 
        /// </summary>
        public FlagSummary() { }


        /// <summary>
        /// Gets the flag id.
        /// </summary>
        public int FlagId { get; private set; }



        /// <summary>
        /// Gets the flagDescription as sComments to pass to the mobile ap
        /// </summary>
        public string sComments
        {
            get
            {
                return this.FlaggedItem != null ? this.FlaggedItem.FlagDescription : string.Empty;
            }
        }

        /// <summary>
        /// Gets the flagged item.
        /// </summary>
        public FlaggedItem FlaggedItem { get; set;}

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns></returns>
        public FlagSummary From(SpendManagementLibrary.Flags.FlagSummary dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.FlagId = dbType.FlagID;

            List<AuthoriserJustification> authoriserJustifications = new List<AuthoriserJustification>();

            if (dbType.FlaggedItem.AuthoriserJustifications != null)
            {
                foreach (var authoriserJustification in dbType.FlaggedItem.AuthoriserJustifications)
                {
                    authoriserJustifications.Add(new AuthoriserJustification().From(authoriserJustification, actionContext));
                }
            }

            FlaggedItem flaggedItem;
            switch (dbType.FlaggedItem.Flagtype)
            {
                case FlagType.Duplicate:
                    flaggedItem = new DuplicateFlaggedItem().From((SpendManagementLibrary.Flags.DuplicateFlaggedItem)dbType.FlaggedItem, actionContext);
                    break;

                case FlagType.GroupLimitWithReceipt:
                case FlagType.GroupLimitWithoutReceipt:
                case FlagType.LimitWithReceipt:
                case FlagType.LimitWithoutReceipt:
                case FlagType.TipLimitExceeded:
                    flaggedItem = new SpendManagementApi.Models.Types.Flags.LimitFlaggedItem().From((SpendManagementLibrary.Flags.LimitFlaggedItem)dbType.FlaggedItem, actionContext);
                    break;

                case FlagType.MileageExceeded:
                case FlagType.NumberOfPassengersLimit:
                case FlagType.HomeToLocationGreater:
                    flaggedItem = new SpendManagementApi.Models.Types.Flags.MileageFlaggedItem().From((SpendManagementLibrary.Flags.MileageFlaggedItem)dbType.FlaggedItem, actionContext);
                    break;

                default:
                    flaggedItem = new FlaggedItem().From(dbType.FlaggedItem, actionContext);
                    break;
            }

            flaggedItem.ExpenseSubcat = dbType.FlaggedItem.ExpenseSubcat;
            flaggedItem.ExpenseDate = dbType.FlaggedItem.ExpenseDate;
            flaggedItem.ExpenseTotal = dbType.FlaggedItem.ExpenseTotal;
            flaggedItem.ExpenseCurrencySymbol = dbType.FlaggedItem.ExpenseCurrencySymbol;      
            flaggedItem.AuthoriserJustifications = authoriserJustifications;

            this.FlaggedItem = flaggedItem;

            return this;
        }

        public SpendManagementLibrary.Flags.FlagSummary To(IActionContext actionContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
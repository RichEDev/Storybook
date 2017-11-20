namespace SpendManagementApi.Models.Types
{
    using SML = SpendManagementLibrary;
    using System;

    using SpendManagementApi.Interfaces;

    /// <summary>
    /// The authoriser justification.
    /// </summary>
    public class AuthoriserJustification : BaseExternalType, IApiFrontForDbObject<SML.Flags.AuthoriserJustification, AuthoriserJustification>
    {

        /// <summary>
        /// Initialises a new instance of the <see cref="AuthoriserJustification"/> class.
        /// </summary>
        public AuthoriserJustification() { }

        /// <summary>
        /// Initialises a new instance of the <see cref="AuthoriserJustification"/> class.
        /// </summary>
        /// <param name="flaggeditemid">
        /// The flaggeditemid.
        /// </param>
        /// <param name="stage">
        /// The stage of the singoff group the justification was given at.
        /// </param>
        /// <param name="fullname">
        /// The full name of the authoriser.
        /// </param>
        /// <param name="justification">
        /// The justification.
        /// </param>
        /// <param name="datestamp">
        /// The datestamp.
        /// </param>
        /// <param name="delegateID">
        /// The employee id of the user providing the justification if a delegate.
        /// </param>
        public AuthoriserJustification(int flaggeditemid, int stage, string fullname, string justification, DateTime datestamp, int? delegateID)
        {
            this.FlaggedItemId = flaggeditemid;
            this.Stage = stage;
            this.FullName = fullname;
            this.Justification = justification;
            this.DateStamp = datestamp;
            this.DelegateID = delegateID;
        }

        #region Properties

        /// <summary>
        /// Gets the flagged item this justification relates to
        /// </summary>
        public int FlaggedItemId { get; private set; }

        /// <summary>
        /// Gets the stage this justification relates to
        /// </summary>
        public int Stage { get; private set; }

        /// <summary>
        /// Gets the full name of the employee that provided the justification
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Gets the justification provided
        /// </summary>
        public string Justification { get; private set; }

        /// <summary>
        /// Gets the date and time the comment was made
        /// </summary>
        public DateTime DateStamp { get; private set; }

        /// <summary>
        /// Gets or sets the id of the employee if a delegate logged the justification.
        /// </summary>
        public int? DelegateID { get; private set; }

        /// <summary>
        /// Gets the elapsed time from now.
        /// </summary>
        public string Time
        {
            get
            {
                TimeSpan span = DateTime.Now - this.DateStamp;
                if (span.Days > 365)
                {
                    int years = span.Days / 365;
                    if (span.Days % 365 != 0)
                    {
                        years += 1;
                    }

                    return string.Format("about {0} {1} ago", years, years == 1 ? "year" : "years");
                }

                if (span.Days > 30)
                {
                    int months = span.Days / 30;
                    if (span.Days % 31 != 0)
                    {
                        months += 1;
                    }

                    return string.Format("about {0} {1} ago", months, months == 1 ? "month" : "months");
                }

                if (span.Days > 0)
                {
                    return string.Format("about {0} {1} ago", span.Days, span.Days == 1 ? "day" : "days");
                }

                if (span.Hours > 0)
                {
                    return string.Format("about {0} {1} ago", span.Hours, span.Hours == 1 ? "hour" : "hours");
                }

                if (span.Minutes > 0)
                {
                    return string.Format("about {0} {1} ago", span.Minutes, span.Minutes == 1 ? "minute" : "minutes");
                }

                if (span.Seconds > 5)
                {
                    return string.Format("about {0} seconds ago", span.Seconds);
                }

                if (span.Seconds <= 5)
                {
                    return "just now";
                }

                return string.Empty;
            }
        }
        #endregion

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public AuthoriserJustification From(SpendManagementLibrary.Flags.AuthoriserJustification dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.DateStamp = dbType.DateStamp;
            this.DelegateID = dbType.DelegateID;
            this.FlaggedItemId = dbType.FlaggedItemID;
            this.FullName = dbType.FullName;
            this.Justification = dbType.Justification;
            this.Stage = dbType.Stage;
            
            return this;

        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Flags.AuthoriserJustification To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }
    }
}
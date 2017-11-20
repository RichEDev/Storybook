using System;

namespace SpendManagementLibrary
{
    using SpendManagementLibrary.Definitions.JoinVia;

    [Serializable()]
    public class cJoin
    {
        /// <summary>
        /// The accountid.
        /// </summary>
        private int nAccountid;

        /// <summary>
        /// The n jointableid.
        /// </summary>
        private Guid nJointableid;

        /// <summary>
        /// The n basetableid.
        /// </summary>
        private Guid nBasetableid;

        /// <summary>
        /// The n destinationtableid.
        /// </summary>
        private Guid nDestinationtableid;

        /// <summary>
        /// The description.
        /// </summary>
        private string sDescription;

        /// <summary>
        /// The dt amended on.
        /// </summary>
        private DateTime dtAmendedOn;

        /// <summary>
        /// Initialises a new instance of the <see cref="cJoin"/> class.
        /// </summary>
        /// <param name="accountid">
        ///     The accountid.
        /// </param>
        /// <param name="jointableid">
        ///     The jointableid.
        /// </param>
        /// <param name="basetableid">
        ///     The basetableid.
        /// </param>
        /// <param name="destinationtableid">
        ///     The destinationtableid.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        /// <param name="lastamendedon">
        ///     The lastamendedon.
        /// </param>
        public cJoin(int accountid, Guid jointableid, Guid basetableid, Guid destinationtableid, string description, DateTime lastamendedon)
        {
            nAccountid = accountid;
            nJointableid = jointableid;
            nBasetableid = basetableid;
            nDestinationtableid = destinationtableid;
            sDescription = description;
            this.steps = new Lazy<JoinSteps>(() => new JoinSteps(this.nAccountid, jointableid));
            dtAmendedOn = lastamendedon;
        }

        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        public Guid jointableid
        {
            get { return nJointableid; }
        }
        public Guid basetableid
        {
            get { return nBasetableid; }
        }
        public Guid destinationtableid
        {
            get { return nDestinationtableid; }
        }
        public string description
        {
            get { return sDescription; }
        }

        /// <summary>
        /// Gets or sets the join steps.
        /// </summary>
        private Lazy<JoinSteps> steps;

        public JoinSteps Steps
        {
            get
            {
                if (this.steps == null)
                {
                    this.steps = new Lazy<JoinSteps>(() => new JoinSteps(this.nAccountid, this.jointableid));
                }

                return this.steps.Value;
            }
        }

        public DateTime amendedon
        {
            get { return dtAmendedOn; }
            set { dtAmendedOn = value; }
        }
        #endregion
    }

}

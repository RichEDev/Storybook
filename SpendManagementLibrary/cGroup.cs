namespace SpendManagementLibrary
{
    using System;

    [Serializable()]
    public class cGroup
    {
        private int _accountId;
        private int _groupId;
        private string _groupname;
        private string _description;
        private bool _oneClickAuthorisation;
        private DateTime _createdon;
        private int _createdby;
        private DateTime _modifiedon;
        private int _modifiedby;
        private GroupStages _groupStages;

        public cGroup()
        {
        }

        public cGroup(int accountid, int groupid, string groupname, string description, bool oneclickauth, DateTime createdon, int createdby, DateTime modifiedon, int modifiedby, SerializableDictionary<int, cStage> stages, bool? notifyClaimantWhenEnvelopeReceived = null, bool? notifyClaimantWhenEnvelopeNotReceived = null)
        {
            this._accountId = accountid;
            this._groupId = groupid;
            this._groupname = groupname;
            this._description = description;
            this._oneClickAuthorisation = oneclickauth;
            this._createdon = createdon;
            this._createdby = createdby;
            this._modifiedon = modifiedon;
            this._modifiedby = modifiedby;
            this._groupStages = new GroupStages(stages);
            this.NotifyClaimantWhenEnvelopeReceived = notifyClaimantWhenEnvelopeReceived;
            this.NotifyClaimantWhenEnvelopeNotReceived = notifyClaimantWhenEnvelopeNotReceived;
        }


        #region properties
        public int accountid
        {
            get { return this._accountId; }
        }
        public int groupid
        {
            get { return this._groupId; }
        }
        public string groupname
        {
            get { return this._groupname; }
        }
        public string description
        {
            get { return this._description; }
        }
        public bool oneclickauthorisation
        {
            get { return this._oneClickAuthorisation; }
        }
        public int stagecount
        {
            get { return stages.Values.Count; }
        }
        public DateTime createdon
        {
            get { return this._createdon; }
        }
        public int createdby
        {
            get { return this._createdby; }
        }
        public DateTime modifiedon
        {
            get { return this._modifiedon; }
        }
        public int modifiedby
        {
            get { return this._modifiedby; }
        }
        public GroupStages stages
        {
            get { return this._groupStages; }
        }

        /// <summary>
        /// Whether to notify the claimant when their envelope is received.
        /// This will only have a value if the account has ReceiptsService enabled, 
        /// and if the group contains the Scan and Attach stage, with the UI value set.
        /// </summary>
        public bool? NotifyClaimantWhenEnvelopeReceived { get; set; }

        /// <summary>
        /// Whether to notify the claimant when their envelope is not received after the configured time has passed.
        /// This will only have a value if the account has ReceiptsService enabled, 
        /// and if the group contains the Scan and Attach stage, with the UI value set.
        /// </summary>
        public bool? NotifyClaimantWhenEnvelopeNotReceived { get; set; }
        
        #endregion
    }
}

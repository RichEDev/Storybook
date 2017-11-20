namespace SpendManagementLibrary
{
    using System;

    [Serializable()]
    public class cAudience
    {
        int nAudienceID;
        string sAudienceName;
        string sDescription;

        public cAudience(int audienceid, string audiencename, string description)
        {
            nAudienceID = audienceid;
            sAudienceName = audiencename;
            sDescription = description;
        }

        #region properties
        public int audienceID
        {
            get { return nAudienceID; }
        }
        public string audienceName
        {
            get { return sAudienceName; }
        }
        public string description
        {
            get { return sDescription; }
        }
        #endregion properties
    }
}

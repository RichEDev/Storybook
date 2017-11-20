using System;
using System.Collections.Generic;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cBroadcastMessage
    {
        private int nBroadcastid;
        private string sTitle;
        private string sMessage;
        private DateTime dtStartdate;
        private DateTime dtEnddate;
        private bool bExpireWhenRead;
        private broadcastLocation blLocation;
        private DateTime dtDatestamp;
        private bool bOncePerSession;
        private DateTime dtCreatedon;
        private int nCreatedby;
        private DateTime dtModifiedon;
        private int nModifiedby;

        public cBroadcastMessage(int broadcastid, string title, string message, DateTime startdate, DateTime enddate, bool expirewhenread, broadcastLocation location, DateTime datestamp, bool oncepersession, DateTime createdon, int createdby, DateTime modifiedon, int modifiedby)
        {
            nBroadcastid = broadcastid;
            sTitle = title;
            sMessage = message;
            dtStartdate = startdate;
            dtEnddate = enddate;
            bExpireWhenRead = expirewhenread;
            blLocation = location;
            dtDatestamp = datestamp;
            bOncePerSession = oncepersession;
            dtCreatedon = createdon;
            nCreatedby = createdby;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;
        }

        #region properties
        public int broadcastid
        {
            get { return nBroadcastid; }
        }
        public string title
        {
            get { return sTitle; }
        }
        public string message
        {
            get { return sMessage; }
        }
        public DateTime startdate
        {
            get { return dtStartdate; }
        }
        public DateTime enddate
        {
            get { return dtEnddate; }
        }
        public bool expirewhenread
        {
            get { return bExpireWhenRead; }
        }
        public broadcastLocation location
        {
            get { return blLocation; }
        }
        public DateTime datestamp
        {
            get { return dtDatestamp; }
        }
        public bool oncepersession
        {
            get { return bOncePerSession; }
        }
        public DateTime createdon
        {
            get { return dtCreatedon; }
        }
        public int createdby
        {
            get { return nCreatedby; }
        }
        public DateTime modifiedon
        {
            get { return dtModifiedon; }
        }
        public int modifiedby
        {
            get { return nModifiedby; }
        }
        #endregion
    }

    [Serializable()]
    public struct sOnlineBroadcastInfo
    {
        public Dictionary<int, cBroadcastMessage> lstonlinebmessages;
        public List<int> lstbmessageids;
        public List<int> lstempreadbroadcasts;
    }

    public enum broadcastLocation
    {
        HomePage = 1,
        SubmitClaim,
        notSet
    }
}

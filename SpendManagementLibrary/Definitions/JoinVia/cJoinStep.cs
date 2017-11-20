namespace SpendManagementLibrary.Definitions.JoinVia
{
    using System;

    [Serializable()]
    public class cJoinStep
    {
        private Guid nJoinBreakdownid;
        private Guid nJointableid;
        private byte bOrder;
        private Guid nDestinationTableid;
        private Guid nSourceTableid;
        private Guid nJoinkey;
        private Guid nDestinationKey;
        private DateTime dtAmendedOn;

        public enum JoinBreakdownSourceType
        {
            Metabase = 0,
            CustomJoinBreakdown = 1
        }

        public cJoinStep(Guid joinbreakdownid, Guid jointableid, byte order, Guid destinationtableid, Guid sourcetableid, Guid joinkey, DateTime amendedon, Guid destinationkey)
        {
            this.nJoinBreakdownid = joinbreakdownid;
            this.nJointableid = jointableid;
            this.bOrder = order;
            this.nDestinationTableid = destinationtableid;
            this.nSourceTableid = sourcetableid;
            this.nJoinkey = joinkey;
            this.dtAmendedOn = amendedon;
            this.nDestinationKey = destinationkey;
        }

        #region properties
        public Guid joinbreakdownid
        {
            get { return this.nJoinBreakdownid; }
        }
        public Guid jointableid
        {
            get { return this.nJointableid; }
        }
        public byte order
        {
            get { return this.bOrder; }
        }
        public Guid destinationtableid
        {
            get { return this.nDestinationTableid; }
        }
        public Guid sourcetableid
        {
            get { return this.nSourceTableid; }
        }
        public Guid joinkey
        {
            get { return this.nJoinkey; }
        }
        public Guid destinationkey
        {
            get { return this.nDestinationKey; }
        }
        public DateTime amendedon
        {
            get { return this.dtAmendedOn; }
        }
        #endregion
    }
}
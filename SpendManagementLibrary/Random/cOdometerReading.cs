using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{

    [Serializable()]
    public class cOdometerReading
    {
        private int nOdometerid;
        private int nCarid;
        private DateTime dtDatestamp;
        private int nOldreading;
        private int nNewreading;
        private DateTime dtCreatedon;
        private int nCreatedby;

        public cOdometerReading()
        {
        }
        public cOdometerReading(int odometerid, int carid, DateTime datestamp, int oldreading, int newreading, DateTime createdon, int createdby)
        {
            nOdometerid = odometerid;
            nCarid = carid;
            dtDatestamp = datestamp;
            nOldreading = oldreading;
            nNewreading = newreading;
            dtCreatedon = createdon;
            nCreatedby = createdby;
        }

        #region properties
        public int odometerid
        {
            get { return nOdometerid; }
        }
        public int carid
        {
            get { return nCarid; }
        }
        public DateTime datestamp
        {
            get { return dtDatestamp; }
        }
        public int oldreading
        {
            get { return nOldreading; }
        }
        public int newreading
        {
            get { return nNewreading; }
        }
        public DateTime createdon
        {
            get { return dtCreatedon; }
        }
        public int createdby
        {
            get { return nCreatedby; }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cProjectCode
    {
        private int nProjectcodeid;
        private string sProjectCode;
        private string sDescription;
        private bool bArchived;

        public cProjectCode(int projectcodeid, string projectcode, string description, bool archived)
        {
            nProjectcodeid = projectcodeid;
            sProjectCode = projectcode;
            sDescription = description;
            bArchived = archived;
        }

        #region properties
        public int projectcodeid
        {
            get { return nProjectcodeid; }
            set { nProjectcodeid = value; }
        }
        public string projectcode
        {
            get { return sProjectCode; }
            set { sProjectCode = value; }
        }
        public string description
        {
            set { sDescription = value; }
            get { return sDescription; }
        }
        public bool archived
        {
            get { return bArchived; }
        }
        #endregion
    }
}

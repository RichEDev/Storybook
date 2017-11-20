using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace SpendManagementLibrary
{
    using SpendManagementLibrary.Definitions.JoinVia;

    [Serializable()]
    public struct sFieldBasics
    {
        private Guid nFieldID;
        private string sDescription;
        private string sFieldType;
        private Guid gTableID;
        private string sFieldName;

        public sFieldBasics(Guid fieldID, string description, string fieldType, string fieldName, Guid tableID)
        {
            nFieldID = fieldID;
            sDescription = description;
            sFieldType = fieldType;
            sFieldName = fieldName;
            gTableID = tableID;
        }

        public sFieldBasics(Guid fieldID, string description, string fieldType, string fieldName)
        {
            nFieldID = fieldID;
            sDescription = description;
            sFieldType = fieldType;
            gTableID = new Guid();
            sFieldName = fieldName;
        }

        public Guid FieldID
        {
            get { return nFieldID; }
        }

        public string Description
        {
            get { return sDescription; }
        }

        public string FieldType
        {
            get { return sFieldType; }
        }

        public Guid TableID
        {
            get { return gTableID; }
        }

        public string FieldName
        {
            get { return sFieldName; }
        }
    }


    [Serializable()]
    public struct sOnlineFieldInfo
    {
        public Dictionary<Guid, cField> lstonlinefields;

        public List<Guid> lstfieldids;
    }


    [Serializable()]
    public struct sOnlineTableInfo
    {
        public Dictionary<Guid, cTable> lstonlinetables;
        public List<Guid> lsttableids;
    }

    [Serializable()]
    public struct sOnlineJoinInfo
    {
        public Dictionary<int, cJoin> lstonlinejoins;
        public List<int> lstjoinids;
        public Dictionary<int, cJoinStep> lstonlinejoinsteps;
        public List<int> lstjoinstepids;
    }

    [Serializable()]
    public class cViewGroup
    {
        private Guid nViewgroupid;
        private string sGroupname;
        private Guid nParentid;
        private int nLevel;
        private DateTime dtAmendedOn;
        SortedList<Guid, cViewGroup> lstChildren;

        public cViewGroup(Guid viewgroupid, string groupname, Guid parentid, int level, DateTime amendedon)
        {
            nViewgroupid = viewgroupid;
            sGroupname = groupname;
            nParentid = parentid;
            nLevel = level;
            dtAmendedOn = amendedon;
        }

        #region Properties

        public Guid viewgroupid
        {
            get { return nViewgroupid; }
        }
        public string groupname
        {
            get { return sGroupname; }
        }
        public Guid parentid
        {
            get { return nParentid; }
        }
        public int level
        {
            get { return nLevel; }
        }
        public DateTime amendedon
        {
            get { return dtAmendedOn; }
        }
        public SortedList<Guid, cViewGroup> children
        {
            get { return lstChildren; }
            set { lstChildren = value; }
        }

        #endregion
    }

    [Serializable()]
    public struct sOnlineViewgroupInfo
    {
        public Dictionary<Guid, cViewGroup> lstonlineviewgroups;
        public List<Guid> lstviewgroupids;
    }

    [Serializable()]
    public struct sFieldToDisplay
    {
        public int fieldid;
        public string code;
        public string description;
        public bool display;
        public bool mandatory;
        public bool individual;
        public bool displaycc;
        public bool mandatorycc;
        public bool displaypc;
        public bool mandatorypc;
        public DateTime amendedon;
    }

    [Serializable()]
    public struct sOnlineAddscreenItemsInfo
    {
        public Dictionary<Guid, cFieldToDisplay> lstonlineaddscreenitems;
        public List<Guid> lstaddscreenids;
    }

    [Serializable()]
    public enum AttachDocumentType
    { 
        None = 0,
        Licence,
        Tax,
        MOT,
        Insurance,
        Service,
        ExpenseReceipt,
        
    }
}

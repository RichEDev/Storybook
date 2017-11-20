using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cGlobalESRElement
    {
        private int nGlobalElementID;
        private string sGlobalElementName;
        private List<cGlobalESRElementField> lstFields;

        public cGlobalESRElement(int GlobalElementID, string GlobalName, List<cGlobalESRElementField> Fields)
        {
            nGlobalElementID = GlobalElementID;
            sGlobalElementName = GlobalName;
            lstFields = Fields;
        }

        #region properties

        public int GlobalElementID
        {
            get { return nGlobalElementID; }
        }
        public string GlobalElementName
        {
            get { return sGlobalElementName; }
        }
        public List<cGlobalESRElementField> Fields
        {
            get { return lstFields; }
        }
        #endregion
    }

    [Serializable()]
    public class cGlobalESRElementField
    {
        private int nGlobalElementFieldID;
        private int nGlobalElementID;
        private string sGlobalElementFieldName;
        private bool bMandatory;
        private bool bControlColumn;

        private readonly bool summaryColumn;

        /// <summary>
        /// Initializes a new instance of the <see cref="cGlobalESRElementField"/> class.
        /// </summary>
        /// <param name="GlobalElementFieldID">
        /// The global element field id.
        /// </param>
        /// <param name="GlobalElementID">
        /// The global element id.
        /// </param>
        /// <param name="GlobalElementFieldName">
        /// The global element field name.
        /// </param>
        /// <param name="Mandatory">
        /// Is this element mandatory.
        /// </param>
        /// <param name="controlColumn">
        /// Is this a control coloum.
        /// </param>
        /// <param name="summaryColumn">
        /// can this column be summarised.
        /// </param>
        /// <param name="rounded">
        /// Can this element be rounded.
        /// </param>
        public cGlobalESRElementField(int GlobalElementFieldID, int GlobalElementID, string GlobalElementFieldName, bool Mandatory, bool controlColumn, bool summaryColumn, bool rounded)
        {
            this.Rounded = rounded;
            this.nGlobalElementFieldID = GlobalElementFieldID;
            this.nGlobalElementID = GlobalElementID;
            this.sGlobalElementFieldName = GlobalElementFieldName;
            this.bMandatory = Mandatory;
            this.bControlColumn = controlColumn;
            this.summaryColumn = summaryColumn;
        }

        #region properties
        public int globalElementFieldID
        {
            get { return nGlobalElementFieldID; }
        }
        public int globalElementID
        {
            get { return nGlobalElementID; }
        }
        public string globalElementFieldName
        {
            get { return sGlobalElementFieldName; }
        }
        public bool Mandatory
        {
            get { return bMandatory; }
        }
        public bool controlColumn
        {
            get { return bControlColumn; }
        }

        public bool SummaryColumn
        {
            get
            {
                return this.summaryColumn;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this column is rounded to integers.
        /// </summary>
        public bool Rounded { get; set; }

        #endregion
    }

    [Serializable()]
    public class cESRElement
    {
        private int nElementID;
        private int nGlobalElementID;
        private List<cESRElementField> lstFields;
        private List<int> lstSubcats;
        private int nNHSTrustID;

        public cESRElement(int ElementID, int GlobalElementID, List<cESRElementField> Fields, List<int> Subcats, int NHSTRustID)
        {
            nElementID = ElementID;
            nGlobalElementID = GlobalElementID;
            lstFields = Fields;
            lstSubcats = Subcats;
            nNHSTrustID = NHSTRustID;
        }

        public cESRElementField getFieldByID(int ID)
        {
            foreach (cESRElementField Field in lstFields)
            {
                if (Field.ElementFieldID == ID)
                {
                    return Field;
                }
            }
            return null;
        }
        
        #region properties
        public int ElementID
        {
            get {return nElementID;}
        }
        public int GlobalElementID
        {
            get { return nGlobalElementID; }
        }
        public List<cESRElementField> Fields
        {
            get { return lstFields; }
        }
        public List<int> Subcats
        {
            get { return lstSubcats; }
        }
        public int NHSTrustID
        {
            get { return nNHSTrustID; }
        }
        #endregion
    }

    [Serializable()]
    public class cESRElementField
    {
        private int nElementFieldID;
        private int nElementID;
        private int nGlobalElementFieldID;
        private Guid gReportColumnID;
        private byte bOrder;
        private Aggregate aAggregate;

        public cESRElementField(int ElementFieldID, int ElementID, int GlobalElementFieldID, Guid ReportColumnID, byte Order, Aggregate Aggregate)
        {
            nElementFieldID = ElementFieldID;
            nElementID = ElementID;
            nGlobalElementFieldID = GlobalElementFieldID;
            gReportColumnID = ReportColumnID;
            bOrder = Order;
            aAggregate = Aggregate;
            
        }

        #region properties
        public int ElementFieldID
        {
            get { return nElementFieldID; }
        }
        public int ElementID
        {
            get { return nElementID; }
        }
        public int GlobalElementFieldID
        {
            get { return nGlobalElementFieldID; }
        }
        public Guid ReportColumnID
        {
            get { return gReportColumnID; }
        }
        public byte Order
        {
            get { return bOrder; }
        }
        public Aggregate Aggregate
        {
            get { return aAggregate; }
        }
        
        #endregion
    }
}

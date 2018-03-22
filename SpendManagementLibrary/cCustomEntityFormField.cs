namespace SpendManagementLibrary
{
    /// <summary>
    /// Class for custom entity field properties at form level
    /// </summary>
    public class cCustomEntityFormField
    {
        private int nFormid;
        private cAttribute clsAttribute;
        private bool bReadOnly;
        private cCustomEntityFormSection clsSection;
        private byte nColumn;
        private byte nRow;
        private string sLabelText;
        private byte nColumnSpan;

        /// <summary>
        /// Mandatory value of an atribute at form level
        /// </summary>
        private bool? fieldMandatoryCheck;

        /// <summary>
        /// Initializes a new instance of the <see cref="cCustomEntityFormField"/> class. 
        /// Initialisation of form fields.
        /// </summary>
        /// <param name="formid">
        /// current form id.
        /// </param>
        /// <param name="attribute">
        /// current attribute.
        /// </param>
        /// <param name="breadonly">
        /// breadcrumb of the form.
        /// </param>
        /// <param name="section">
        /// sections in a form.
        /// </param>
        /// <param name="column">
        /// columns in a form.
        /// </param>
        /// <param name="row">
        /// rows in a form.
        /// </param>
        /// <param name="labeltext">
        /// Label of an attribute.
        /// </param>
        /// <param name="isMandatory">
        /// Mandatory check of an attribute at form level.
        /// </param>
        /// <param name="defaultValue">
        /// The default Value text.
        /// </param>
        /// <param name="columnSpan">
        /// columnSpan.
        /// </param>
        public cCustomEntityFormField(int formid, cAttribute attribute, bool breadonly, cCustomEntityFormSection section, byte column, byte row, string labeltext = "", bool? isMandatory = null, string defaultValue = null, byte columnSpan = 1)
        {
            this.nFormid = formid;
            this.clsAttribute = attribute;
            this.bReadOnly = breadonly;
            this.clsSection = section;
            this.nColumn = column;
            this.nRow = row;
            this.sLabelText = labeltext;
            this.nColumnSpan = columnSpan;
            this.DefaultValue = defaultValue;
            this.fieldMandatoryCheck = isMandatory;
        }

        #region properties
        public int formid
        {
            get { return this.nFormid; }
        }
        public cAttribute attribute
        {
            get { return this.clsAttribute; }
        }
        public bool isReadOnly
        {
            get { return this.bReadOnly; }
        }
        public cCustomEntityFormSection section
        {
            get { return this.clsSection; }
        }
        public byte column
        {
            get { return this.nColumn; }
        }
        public byte row
        {
            get { return this.nRow; }
        }
        public string labelText
        {
            get { return this.sLabelText; }
        }

        /// <summary>
        /// Gets attributes mandatory status at form level
        /// </summary>
        public bool? IsMandatory
        {
            get { return this.fieldMandatoryCheck; }
        }
        public byte columnSpan
        {
            get
            {
                byte columnSpan = 1;

                if (this.attribute.GetType() == typeof(cCommentAttribute) || this.attribute.GetType() == typeof(cSummaryAttribute) || this.attribute.GetType() == typeof(cOneToManyRelationship))
                {
                    columnSpan = 2;
                }

                if (this.attribute.GetType() == typeof(cTextAttribute))
                {
                    switch (((cTextAttribute)this.attribute).format)
                    {
                        case AttributeFormat.FormattedText:
                            columnSpan = 2;
                            break;
                        case AttributeFormat.MultiLine:
                            columnSpan = 2;
                            break;
                        case AttributeFormat.SingleLineWide:
                            columnSpan = 2;
                            break;
                        default:
                            break;
                    }
                }

                if (this.attribute.GetType() == typeof(cListAttribute))
                {
                    if (((cListAttribute)this.attribute).format == AttributeFormat.ListWide)
                    {
                        columnSpan = 2;
                    }
                }

                return columnSpan;
            }
        }

        /// <summary>
        /// Gets the default value for the form field. This needs updating so we can have different types of default value.
        /// </summary>
        public string DefaultValue { get; private set; }

        #endregion
    }
}
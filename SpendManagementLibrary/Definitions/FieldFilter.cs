using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    using SpendManagementLibrary.Definitions.JoinVia;

    /// <summary>
	/// A filter to be applied to a field
	/// </summary>
	public class FieldFilter
    {
        private cField _field;
        private ConditionType _conditionType;
        private readonly string _valueOne;
        private readonly string _valueTwo;
        private readonly byte _order;
        private readonly JoinVia _joinVia;
        private readonly int _formId;
        private readonly ConditionJoiner _joiner;

        /// <summary>
        /// Is this parent child relationship filter.
        /// </summary>
        private readonly bool _isParentFilter;

	    /// <summary>
	    /// Is this filter added on edit form.
	    /// </summary>
	    private readonly bool _isFilterOnEdit;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="field">
        /// Field to filter
        /// </param>
        /// <param name="conditiontype">
        /// Filter condition
        /// </param>
        /// <param name="valueOne">
        /// First filter value
        /// </param>
        /// <param name="valueTwo">
        /// Second filter value
        /// </param>
        /// <param name="order">
        /// Order of filter
        /// </param>
        /// <param name="joinVia">
        /// Join used to build filter
        /// </param>
        /// <param name="formid">
        /// Form to which filter should apply
        /// </param>
        /// <param name="joiner">
        /// Join condition.
        /// </param>
        /// <param name="isParentFilter">
        /// Is this parent child relationship filter
        /// </param>
        /// <param name="isFilterOnEdit">
        /// Filter added on edit
        /// </param>
        public FieldFilter(cField field, ConditionType conditiontype, string valueOne, string valueTwo, byte order, JoinVia joinVia, int formid=0, ConditionJoiner joiner = ConditionJoiner.And, bool isParentFilter = false, bool isFilterOnEdit = false)
		{
            this._field = field;
            this._conditionType = conditiontype;
            this._valueOne = valueOne;
            this._valueTwo = valueTwo ?? string.Empty;
            this._order = order;
            this._joinVia = joinVia;
            this._joiner = joiner;
            this._formId = formid;
            this._isParentFilter = isParentFilter;
		    this._isFilterOnEdit = isFilterOnEdit;
		}

        #region properties

        /// <summary>
        /// The field to filter using
        /// </summary>
        public cField Field
        {
            get { return _field; }
            set { _field = value; }
        }
        /// <summary>
        /// The expression operator to use
        /// </summary>
        public ConditionType Conditiontype
        {
            get { return _conditionType; }
            set { _conditionType = value; }
        }
        /// <summary>
        /// The first criterion, required
        /// </summary>
        public string ValueOne
        {
            get { return _valueOne; }
        }
        /// <summary>
        /// The second criterion, only required with some operators - eg. between
        /// </summary>
        public string ValueTwo
        {
            get { return _valueTwo; }
        }
        /// <summary>
        /// The order in which the filters should be applied
        /// </summary>
        public byte Order
        {
            get { return _order; }
        }
        /// <summary>
        /// A join path to use with the field
        /// </summary>
        public JoinVia JoinVia
        {
            get { return _joinVia; }
        }

        /// <summary>
		/// A formid in which the filters should be applied
		/// </summary>
        public int FormId
        {
            get { return _formId; }
        }
        public ConditionJoiner Joiner
        {
            get
            {
                return this._joiner;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is parent filter.
        /// </summary>
        public bool IsParentFilter => this._isParentFilter;

	    /// <summary>
	    /// Gets a value indicating whether filter added on edit.
	    /// </summary>
	    public bool IsFilterOnEdit => this._isFilterOnEdit;

        #endregion
    }

    /// <summary>
    /// A stripped-down, serializable field filter object
    /// </summary>
    public class JSFieldFilter
    {
        /// <summary>
        /// Required for serialisation
        /// </summary>
        public JSFieldFilter()
        {
        }

        /// <summary>
        /// Field Guid
        /// </summary>
        public Guid FieldID = Guid.Empty;

        /// <summary>
        /// Condition Type
        /// </summary>
        public ConditionType ConditionType;

        /// <summary>
        /// Value One
        /// </summary>
        public string ValueOne = "";

        /// <summary>
        /// Value Two
        /// </summary>
        public string ValueTwo = "";

        /// <summary>
        /// Order
        /// </summary>
        public byte Order;

        /// <summary>
        /// JoinVia Guid
        /// </summary>
        public int JoinViaID = 0;

        public ConditionJoiner Joiner;
        /// <summary>
		/// Form Id
		/// </summary>
	    public int formID = 0;

	    /// <summary>
	    /// Is this parent child relationship filter.
	    /// </summary>
	    public bool IsParentFilter;

        /// <summary>
        /// Filter added on edit.
        /// </summary>
	    public bool FilterOnEdit;

	}


    public enum FieldFilterProductArea
    {
        Views = 0,
        Attributes = 1,
        UserDefinedFields = 2
    }
}

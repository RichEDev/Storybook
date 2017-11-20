using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SpendManagementApi.Interfaces;
using SpendManagementApi.Models.Types.Employees;
using SpendManagementApi.Models.Types.Expedite;
using SpendManagementLibrary;

namespace SpendManagementApi.Models.Types
{
    /// <summary>
    /// Hold the data realting to a User Defined Field
    /// </summary>
    public class UserDefinedFieldType : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.cUserDefinedField, UserDefinedFieldType>
    {
        /// <summary>
        /// The unique Id of the user defined field
        /// </summary>
        public int UserDefinedFieldId { get; set; }

        /// <summary>
        /// The attribute data that is associated
        /// </summary>
        public Attribute Attribute { get; set; }

        /// <summary>
        /// The value of the user defined field
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// The order of the user defined field
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an employee can populate the user defined field.
        /// </summary>
        public bool AllowEmployeeToPopulate { get; set; }

        /// <summary>
        /// Transforms a <see cref="cUserDefinedField">cUserDefinedField</see> to a API type of <see cref="UserDefinedFieldType">UserDefinedFieldType</see>
        /// </summary>
        /// <param name="dbType">The a <see cref="cUserDefinedField">cUserDefinedField</see></param>
        /// <param name="actionContext">The action context</param>
        /// <returns></returns>
        public UserDefinedFieldType From(cUserDefinedField dbType, IActionContext actionContext)
        {
            this.UserDefinedFieldId = dbType.userdefineid;
            this.Attribute = new Attribute().From(dbType.attribute, actionContext);
            this.AllowEmployeeToPopulate = dbType.AllowEmployeeToPopulate;
            return this;
        }

        public cUserDefinedField To(IActionContext actionContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
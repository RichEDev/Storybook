using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auto_Tests
{
    /// <summary>
    /// Used to give the attributes database column names a more readable description 
    /// </summary>
    public enum SortAttributesByColumn
    {
        [System.ComponentModel.Description("display_name")]
        DisplayName,
        [System.ComponentModel.Description("description")]
        Description,
        [System.ComponentModel.Description("fieldtype")]
        FieldType,
        [System.ComponentModel.Description("is_audit_identity")]
        UsedForAudit
    }
}

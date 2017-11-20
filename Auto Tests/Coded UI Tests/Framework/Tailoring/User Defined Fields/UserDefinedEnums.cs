using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auto_Tests
{
    public enum SortByColumn
    {
        [System.ComponentModel.Description("Display_Name")]
        DisplayName,
        [System.ComponentModel.Description("Description")]
        Description,
        [System.ComponentModel.Description("FieldType")]
        FieldType,
        [System.ComponentModel.Description("Mandatory")]
        Mandatory,
        [System.ComponentModel.Description("AppliesTo")]
        AppliesTo
    }
}

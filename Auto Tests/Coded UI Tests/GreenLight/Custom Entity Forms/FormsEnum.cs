using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;


namespace Auto_Tests
{
    /// <summary>
    /// Used to give the custom entity database column names a more readable description 
    /// </summary>
    public enum SortFormsByColumn
    {
        [System.ComponentModel.Description("form_name")]
        FormName,
        [System.ComponentModel.Description("description")]
        Description
    }
}

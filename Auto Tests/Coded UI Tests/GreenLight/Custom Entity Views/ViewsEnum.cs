using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Views
{
    /// <summary>
    /// Used to give the custom entity database column names a more readable description 
    /// </summary>
    public enum SortViewsByColumn
    {
        [System.ComponentModel.Description("view_name")]
        ViewName,
        [System.ComponentModel.Description("description")]
        Description
    }
}

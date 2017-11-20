using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auto_Tests
{
    /// <summary>
    /// Used to give the custom entity database column names a more readable description 
    /// </summary>
    public enum SortCustomEntitiesByColumn
    {
        [System.ComponentModel.Description("entity_name")]
        EntityName, 
        [System.ComponentModel.Description("description")]
        Description
    }
}

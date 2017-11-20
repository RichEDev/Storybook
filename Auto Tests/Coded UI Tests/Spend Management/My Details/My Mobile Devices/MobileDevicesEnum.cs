using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auto_Tests
{
    /// <summary>
    /// Used to give the attributes database column names a more readable description 
    /// </summary>
    public enum SortMobileDevicesByColumn
    {
        [System.ComponentModel.Description("deviceName")]
        Name,
        [System.ComponentModel.Description("deviceTypeID")]
        Type,
        [System.ComponentModel.Description("pairingKey")]
        PairingKey
    }
}

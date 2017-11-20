using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Auto_Tests.Tools
{
    public class EnumHelper
    {
        public enum TableSortOrder
        {
            [System.ComponentModel.Description("ASC")]
            ASC = 1,
            [System.ComponentModel.Description("DESC")]
            DESC = 2
        }

        public static String GetEnumDescription(Enum e)
        {
            FieldInfo fieldInfo = e.GetType().GetField(e.ToString());
            System.ComponentModel.DescriptionAttribute[] enumAttributes = (System.ComponentModel.DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (enumAttributes.Length > 0)
            {
                return enumAttributes[0].Description;
            }
            return e.ToString();
        }
    }
}

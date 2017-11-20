using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    public class cView
    {
        private SortedList<byte, cField> lstFields;

        public cView(SortedList<byte, cField> fields)
        {
            lstFields = fields;
        }
    }
}

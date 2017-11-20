using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.WebUI.CalcEngine;

namespace SpendManagementLibrary
{
    public class cExcel : Infragistics.WebUI.CalcEngine.UltraCalcUserDefinedFunction
    {
        public override string Name
        {
            get { return "EXCEL"; }
        }

        public override int MaxArgs
        {
            get { return 1; }
        }

        public override int MinArgs
        {
            get { return 1; }
        }

        public override string[] ArgDescriptors
        {
            get { return new string[] {"Value." }; }
        }

        public override string[] ArgList
        {
            get { return new string[] { "value" }; }
        }

        public override string Category
        {
            get { return "Engineering"; }
        }

        public override string Description
        {
            get { return "Does something"; }
        }

        protected override Infragistics.WebUI.CalcEngine.UltraCalcValue Evaluate(UltraCalcNumberStack numberStack, int argumentCount)
        {
            string value = numberStack.Pop().ToString();
            
            UltraCalcValue a = new UltraCalcValue("=" + value);

            return a;
        }
    }
}

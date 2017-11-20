using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infragistics.WebUI.CalcEngine;

namespace SpendManagementLibrary
{
    public class cAddressFunction : UltraCalcUserDefinedFunction
    {
        public override string[] ArgDescriptors
        {
            get { return new string[] {"The row.", "The column." }; }
        }

        public override string[] ArgList
        {
            get { return new string[] {"row", "column" }; }
        }

        public override string Category
        {
            get { return "TextAndData"; }
        }

        public override string Description
        {
            get { return "Get the cell value from the row and column references"; }
        }

        protected override Infragistics.WebUI.CalcEngine.UltraCalcValue Evaluate(UltraCalcNumberStack numberStack, int argumentCount)
        {
            object val1 = numberStack.Pop();
            object val2 = numberStack.Pop();

            return new UltraCalcValue("ADDRESS(\"" + val2 + "\",\"" + val1 + "\")");
        }

        public override int MaxArgs
        {
            get { return 2; }
        }

        public override int MinArgs
        {
            get { return 2; }
        }

        public override string Name
        {
            get { return "ADDRESS"; }
        }
    }
}

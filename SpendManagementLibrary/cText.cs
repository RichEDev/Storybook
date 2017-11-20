using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.WebUI.CalcEngine;

namespace SpendManagementLibrary
{
    using System.Globalization;

    public class cText : Infragistics.WebUI.CalcEngine.UltraCalcUserDefinedFunction
    {
        public override string[] ArgDescriptors
        {
            get { return new string[] { "Value is a numeric value.", "Format_text is a numeric format as a text string enclosed in quotation marks." }; }
        }

        public override string[] ArgList
        {
            get { return new string[] { "value", "format" }; }
        }

        public override string Category
        {
            get { return "TextAndData"; }
        }

        public override string Description
        {
            get { return "Converts a value to text in a specific number format."; }
        }

        protected override UltraCalcValue Evaluate(UltraCalcNumberStack numberStack, int argumentCount)
        {
            UltraCalcValue format = numberStack.Pop();
            UltraCalcValue value = numberStack.Pop();
            double numbers;
            if (double.TryParse(value.ToString(), out numbers))
            {
                string val = numbers.ToString(format.ToString());
                UltraCalcValue a = new UltraCalcValue(val);
                return a;
            }

            DateTime date;
            System.Globalization.DateTimeFormatInfo dateTimeFormatInfo = System.Globalization.DateTimeFormatInfo.CurrentInfo;
            if (DateTime.TryParse(value.ToString(dateTimeFormatInfo), out date))
            {
                string formattedDate = date.ToString(format.ToString());
                UltraCalcValue ultraVal = new UltraCalcValue(formattedDate);
                return ultraVal;
            }

            return new UltraCalcValue("TEXT(\"" + value.Value + "\",\"" + format.Value + "\")");
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
            get { return "TEXT"; }
        }
    }
}

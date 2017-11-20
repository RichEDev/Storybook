﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infragistics.WebUI.CalcEngine;

namespace SpendManagementLibrary
{
    public class cColumnFunction : UltraCalcUserDefinedFunction
    {
        public override string[] ArgDescriptors
        {
            get { return new string[] { }; }
        }

        public override string[] ArgList
        {
            get { return new string[] { }; }
        }

        public override string Category
        {
            get { return "TextAndData"; }
        }

        public override string Description
        {
            get { return "Get the current column number"; }
        }

        protected override Infragistics.WebUI.CalcEngine.UltraCalcValue Evaluate(UltraCalcNumberStack numberStack, int argumentCount)
        {
            return new UltraCalcValue("COLUMN()");
        }

        public override int MaxArgs
        {
            get { return 0; }
        }

        public override int MinArgs
        {
            get { return 0; }
        }

        public override string Name
        {
            get { return "COLUMN"; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

namespace Auto_Tests.Tools
{

    public class ValidationAsterisk
    {
        /// <summary>
        /// Static method that verifies whether the red asterisk is shown on the page
        /// 
        /// The span is the element that contains the asterisk
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static Boolean IsValidationAsteriskShown(HtmlSpan span)
        {
            return !(span.ControlDefinition.ToLower().Contains("display: none") || span.ControlDefinition.ToLower().Contains("visibility: hidden"));
        }
    }
}

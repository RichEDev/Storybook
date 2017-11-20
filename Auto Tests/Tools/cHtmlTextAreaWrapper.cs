using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting;
using System.Text.RegularExpressions;


namespace Auto_Tests.Tools
{
    public class cHtmlTextAreaWrapper : HtmlTextArea
    {
     
        private int? maxLength = null;
        private string controlId;
        private string pageSource;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pageSource">HtmlMarkup</param>
        /// <param name="idOfControl">id string of the control to locate within the page source</param>
        /// <param name="parentId">Parent control that the id string is located within</param>
        public cHtmlTextAreaWrapper(string pageSource, string idOfControl, HtmlControl parentId) : base (parentId)
        {
                ValidateArguments(pageSource, idOfControl);
                controlId = idOfControl;
                this.pageSource = pageSource;
        }


        /// <summary>
        /// Validates input arguments are valid.
        /// Page source - The html document markup source
        /// IdofControl - ControlId to find in the page source
        /// </summary>
        /// <param name="pageSource"></param>
        /// <param name="idOfControl"></param>
        private static void ValidateArguments(string pageSource, string idOfControl)
        {
            if (string.IsNullOrEmpty(idOfControl))
            {
                throw new ArgumentNullException("Id of control cannot be null or empty!!");
            }
            if (string.IsNullOrEmpty(pageSource))
            {
                throw new ArgumentNullException("pageSource cannot be null or empty!!");
            }
        }

        /// <summary>
        /// Returns max length if found else returns null
        /// </summary>
        /// <returns></returns>
        public int? GetMaxLength() 
        {
            if (maxLength == null)
            {
                maxLength = ExtractMaxLengthForTextArea();
            }
            return maxLength;
        }

        /// <summary>
        /// Extracts the maxlength based on the htmlmarkup page for the control id provided in
        /// the constructor
        /// returns null if not found else returns maxlength
        /// </summary>
        /// <returns></returns>
        private int? ExtractMaxLengthForTextArea()
        {
            Match match = Regex.Match(pageSource, "id=" + controlId + ".*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (match.Success)
            {
              //textareamaxlength=.*?(?<maxlength>[0-9]+)
               string textAreaMaxLengthString = match.Groups[0].Value;
               match= Regex.Match(textAreaMaxLengthString, "textareamaxlength=.*?(?<maxlength>[0-9]+)");
               if (match.Success)
               {
                   maxLength = Convert.ToInt32(match.Groups["maxlength"].Value);
               }
              
            }
                return maxLength;
        }      
    }
}

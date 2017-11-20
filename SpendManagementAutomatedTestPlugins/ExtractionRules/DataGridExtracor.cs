using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.WebTesting;

namespace SpendManagement.AutomatedTests.Runtime.ExtractionRules
{
    /// <summary>
    /// 
    /// </summary>
    public class DataGridIdExtract : ExtractionRule
    {
        #region Private Constants

        /// <summary>
        /// {0} => The Tag We're Looking For (i.e. Table)
        /// {1} => The Sub-Expression (Greedy)
        /// </summary>
        private const string RegexTemplate = "<(?<tag>{0})\\b{1}>(?<data>.+)</\\k<tag>>";
        //private const string RegexTemplate = "<(?<tag>\\w+)\\b{SUB}>(?<data>.+)</\\k<tag>>";

        /// <summary>
        /// {0} => The ID Of The Tag
        /// {1} => The Css Class Of The Tag
        /// </summary>
        private const string RegexSubTemplate = "(?>\\s+(?:id=\"(?<id>{0})\"|class=\"(?<class>{1})\")|[^\\s>]+|\\s+)+";
        //private const string RegexSubTemplate = "(?>\\s+(?:id=\"(?<id>[^\"]*)\"|class=\"(?<class>[^\"]*)\")|[^\\s>]+|\\s+)*";

        #endregion

        #region Private Fields

        /// <summary>
        /// Stores the name of the HTML id/name of the grid control
        /// </summary>
        private string _gridControlId;

        /// <summary>
        /// Stores the name of the html tag to search for
        /// </summary>
        private string _tagName;

        /// <summary>
        /// Stores the name of the table element to search for
        /// </summary>
        private string _elementName;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or Sets the outputed HTML id/name of the grid control that will be used to search for.
        /// </summary>
        [DisplayName("Grid Control ID")]
        [Description("Sets the control id from which to extract from.")]
        public string GridControlID
        {
            get
            {
                return this._gridControlId;
            }
            set
            {
                this._gridControlId = value;
            }
        }

        /// <summary>
        /// Gets or Sets the HTML tag that the extractor will use to extract data from.
        /// </summary>
        [DisplayName("Tag Name")]
        [Description("The HTML tag that the extractor will use to extract the data from.")]
        public string TagName
        {
            get
            {
                return this._tagName;
            }
            set
            {
                this._tagName = value;
            }
        }

        /// <summary>
        /// Gets or Sets the HTML tag attribute that the extractor will extract the value from.
        /// </summary>
        [DisplayName("Element Name")]
        [Description("The element in the grid that you are searching for the ID of")]
        public string ElementName
        {
            get
            {
                return this._elementName;
            }
            set
            {
                this._elementName = value;
            }
        }

        #endregion

        /// <summary>
        /// Performs the actual extraction process on the resultant HTML response
        /// in the form of a HtmlDocument
        /// </summary>
        /// <param name="sender">The currently running WebTest</param>
        /// <param name="e">Event data</param>
        public override void Extract(object sender, ExtractionEventArgs e)
        {
            this.ValidateParameters();

            try
            {
                string rawHtml = e.Response.BodyString;
                string regexSub = String.Format(DataGridIdExtract.RegexSubTemplate, new Object[] { this.TagName, "" });
                string regex = String.Format(DataGridIdExtract.RegexTemplate, new Object[] { this.TagName, regexSub });

                Match gridMatch = Regex.Match(rawHtml, regex, RegexOptions.Singleline);

                if ((gridMatch.Success) && (gridMatch.Groups["data"].Success))
                {
                    // Right now we have our table so all we have to do now is
                    // enumerate through all the rows to find our ID :-)
                    string tableData = gridMatch.Groups["data"].Value;
                    foreach (string row in Regex.Split(tableData, "</tr>", RegexOptions.Singleline))
                    {
                        if (row.Contains(this.ElementName))
                        {
                            Match idMatch = Regex.Match(row, "(?:\\=+|\\(+)(\\p{N}+)(?:\\)?)", RegexOptions.Singleline);
                            if (idMatch.Success)
                            {
                                int outputId = 0;
                                int.TryParse(idMatch.Value.Trim(new char[] { '(', ')', '=', ' ', ',' }), out outputId);

                                if (outputId > 0)
                                {
                                    e.Success = true;
                                    e.Message = "Element Located";
                                    e.WebTest.Context.Add(this.ContextParameterName, outputId);

                                    return;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    e.Success = false;
                    e.Message = "Failed to locate element in grid";
                }
                else
                {
                    e.Message = "The specified grid was not found in the response.";
                    e.Success = false;
                }
            }
            catch (Exception ex)
            {
                e.Message = ex.Message;
                e.Success = false;
            }
        }

        /// <summary>
        /// Validates that all extraction properties required are set
        /// before the extraction event takes place.
        /// </summary>
        private void ValidateParameters()
        {
            if (this._elementName == null)
            {
                throw new InvalidOperationException("ElementName");
            }

            if (this._gridControlId == null)
            {
                throw new InvalidOperationException("GridControlID");
            }

            if (this._tagName == null)
            {
                throw new InvalidOperationException("TagName");
            }
        }
    }
}

namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Web.Script.Services;
    using System.Web.Services;

    using SpendManagementLibrary;

    /// <summary>
    /// Summary description for svcTooltip
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    
    [ScriptService]
    public class svcTooltip : WebService
    {
        /// <summary>
        /// The get tool tip contents.
        /// </summary>
        /// <param name="tooltipID">
        /// The tooltip id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetToolTipContents(string tooltipID,string tooltipArea)
        {
            cHelp clshelp;
            string contents = string.Empty;
            if (User.Identity.Name == string.Empty)
            {
                clshelp = new cHelp();
            }
            else
            {
                CurrentUser user = cMisc.GetCurrentUser();
                clshelp = new cHelp(user.AccountID);
            }

            cHelpItem helpitem = clshelp.GetToolTipByIntAndArea(tooltipID + "," + tooltipArea);

            if (helpitem != null)
            {
                contents = helpitem.helptext;
            }

            return contents;
        }

        /// <summary>
        /// The get tooltip.
        /// </summary>
        /// <param name="contextKey">
        /// The context key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string getTooltip(string contextKey)
        {
            cHelp clshelp;

            if (User.Identity.Name == string.Empty)
            {
                clshelp = new cHelp();
            }
            else
            {
                CurrentUser user = cMisc.GetCurrentUser();
                clshelp = new cHelp(user.AccountID);
            }
            
            cHelpItem helpitem = clshelp.GetToolTipByIntAndArea(contextKey);

            return this.addIFrame(helpitem.helptext);
        }

        /// <summary>
        /// The get user defined tooltip.
        /// </summary>
        /// <param name="contextKey">
        /// The context key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string getUserDefinedTooltip(string contextKey)
        {
            string[] arrid = contextKey.Split(',');
            int id = Convert.ToInt32(arrid[0]);
            int accountid = Convert.ToInt32(arrid[1]);
            var clsuserdefined = new cUserdefinedFields(accountid);
            cUserDefinedField clsfield = clsuserdefined.GetUserDefinedById(id);

            return this.addIFrame(clsfield.tooltip);
        }

        /// <summary>
        /// Adds an IFrame to the tooltip to cater for IE 6.5 and belows bad select object
        /// </summary>
        /// <param name="tooltipString"></param>
        /// <returns></returns>
        private string addIFrame(string tooltipString)
        {
             return "<div class=\"stooltipcontent\">" + tooltipString + "</div><!--[if lte IE 6.5]><iframe class=\"iframe_ie6_tooltip\" src=\"/blank.htm\"></iframe><![endif]--></div></div>"; 
        }

        /// <summary>
        /// The get transaction.
        /// </summary>
        /// <param name="contextKey">
        /// The context key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string getTransaction(string contextKey)
        {
            int transactionid = Convert.ToInt32(contextKey);
            CurrentUser user = cMisc.GetCurrentUser();
            var clsstatements = new cCardStatements(user.AccountID);
            cCardTransaction transaction = clsstatements.getTransactionById(transactionid);
            cCardStatement statement = clsstatements.getStatementById(transaction.statementid);
            var output = new StringBuilder();

            output.Append("<table>");
            output.Append("<tr>");
            output.Append("<td class=\"labeltd\">Transaction Date:</td>");
            output.Append("<td class=\"inputtd\">");
            if (transaction.transactiondate != null)
            {
                output.Append(((DateTime)transaction.transactiondate).ToShortDateString() + "</td>");
            }

            output.Append("</tr>");
            output.Append("<tr>");
            output.Append("<td class=\"labeltd\">Description:</td>");
            output.Append("<td class=\"inputtd\">" + transaction.description + "</td>");
            output.Append("</tr>");
            
            var clscurrencies = new cGlobalCurrencies();
            if (transaction.globalcurrencyid != null && transaction.globalcurrencyid != 0)
            {
                cGlobalCurrency currency = clscurrencies.getGlobalCurrencyById((int)transaction.globalcurrencyid);
                output.Append("<tr>");
                output.Append("<td class=\"labeltd\">Currency:</td>");
                output.Append("<td class=\"inputtd\">" + currency.label + "</td>");
                output.Append("</tr>");
            }

            output.Append("<tr>");
            output.Append("<td class=\"labeltd\">Transaction Amount:</td>");
            output.Append("<td class=\"inputtd\">" + transaction.transactionamount.ToString("###,###,##0.00") + "</td>");
            output.Append("</tr>");
            output.Append("<tr>");
            output.Append("<td class=\"labeltd\">Original Amount:</td>");
            output.Append("<td class=\"inputtd\">" + transaction.originalamount.ToString("###,###,##0.00") + "</td>");
            output.Append("</tr>");

            output.Append("<tr>");
            output.Append("<td class=\"labeltd\">Exchange Rate:</td>");
            output.Append("<td class=\"inputtd\">" + transaction.exchangerate + "</td>");
            output.Append("</tr>");

            var clstemplates = new cCardTemplates(user.AccountID);
            cCardTemplate template = clstemplates.getTemplate(statement.Corporatecard.cardprovider.cardprovider);
            if (template != null)
            {
                cCardTemplateField field = null;
                cCardRecordType recType = template.RecordTypes[CardRecordType.CardTransaction];

                foreach (KeyValuePair<string, object> i in transaction.moredetails)
                {
                    field = null;
                    field = recType.getFieldByName(i.Key);

                    if (field != null)
                    {
                        if (field.displayinmoredetailstable)
                        {
                            output.Append("<tr>");
                            output.Append("<td class=\"labeltd\">" + field.label + ":</td>");
                            output.Append("<td class=\"inputtd\">" + i.Value + "</td>");
                            output.Append("</tr>");
                        }
                    }
                }
            }
            else
            {
                output.Append("<tr>");
                output.Append("<td class=\"inputtd\" colspan=\"2\" align=\"center\">Could not load further information. There was a problem loading the card template.</td>");
                output.Append("</tr>");
            }

            output.Append("</table>");
            return output.ToString();
        }
    }
}

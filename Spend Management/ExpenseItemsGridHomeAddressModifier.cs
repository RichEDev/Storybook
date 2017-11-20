using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using SpendManagementLibrary;
using SpendManagementLibrary.Employees;

namespace Spend_Management
{
    /// <summary>
    /// For data protection purposes:
    /// Goes through the HTML grid for the claim viewer,  looks at the 'from address ID' column
    /// and if it is the employee's home address, sets the 'from address' (text) column to 'Home'.
    /// Then does same with to.
    /// Then removes the ID columns.
    /// </summary>
    public class ExpenseItemsGridHomeAddressModifier
    {
        private EmployeeHomeAddresses homeAddresses;
        private cAccountProperties accountProperties;

        public ExpenseItemsGridHomeAddressModifier(int accountId, int employeeId)
        {
            cEmployees employees = new cEmployees(accountId);
            var employee = employees.GetEmployeeById(employeeId);
            homeAddresses = employee.GetHomeAddresses();

            CurrentUser currentUser = cMisc.GetCurrentUser();
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(accountId);
            cAccountSubAccount subAccount = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId);
            accountProperties = subAccount.SubAccountProperties.Clone();
        }

        public string ReplaceTextFieldWithHomeIfIdFieldIsHomeAddress(string html, cField idField, string textFieldName)
        {
            var htmldoc = new HtmlDocument();
            htmldoc.LoadHtml(html);
            
            HtmlNode headerRow = htmldoc.DocumentNode.SelectNodes(".//tr[th]").FirstOrDefault(); //all trs that have th children
            if(headerRow != null)
            {
                HtmlNodeCollection headers = headerRow.SelectNodes("th");
                var headerTexts = headers.Select(GetHeaderText).ToArray();
                var idFieldIndex = Array.IndexOf(headerTexts, idField.Description);
                var textFieldIndex = Array.IndexOf(headerTexts, textFieldName);
                if(idFieldIndex != -1 && textFieldIndex != -1)
                {
                    foreach (var row in htmldoc.DocumentNode.SelectNodes(".//tr")) //all trs (need to do it for header row as well so don't limit to tr[td])
                    {
                        var tds = row.SelectNodes("td");
                        if (tds != null && tds.Count == headers.Count) 
                        {
                            int addressId;
                            if (int.TryParse("0" + tds[idFieldIndex].InnerText, out addressId) && homeAddresses.HomeLocations.Any(h => h.LocationID == addressId))
                            {
                                tds[textFieldIndex].RemoveAllChildren();
                                tds[textFieldIndex].AppendChild(htmldoc.CreateTextNode(accountProperties.HomeAddressKeyword));
                            }
                    
                            tds[idFieldIndex].Remove();
                        }

                        var ths = row.SelectNodes("th");
                        if (ths != null && ths.Count == headers.Count)
                        {
                            ths[idFieldIndex].Remove();
                        }
                    }
                }
                
            }
            return htmldoc.DocumentNode.OuterHtml;
        }

        private string GetHeaderText(HtmlNode th)
        {
            string headerText = "";
            var a = th.SelectSingleNode("a");
            if (a != null) //doesn't matter if the th hasn't got an a tag in it, as it hasn't for the edit and delete cols
            {
                headerText = a.InnerText;
            }
            return headerText;
        }
    }
}
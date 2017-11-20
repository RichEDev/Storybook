using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HtmlAgilityPack;
using SpendManagementLibrary;
using SpendManagementLibrary.Addresses;
using SpendManagementLibrary.Employees;
using SpendManagementLibrary.Mileage;
using Spend_Management;

/// <summary>
/// Builds the HTML for the Mileage Grid
/// </summary>
public class MileageGridBuilder
{
    private string html;

    public MileageGridBuilder()
    {
        html = LoadHtml();
    }

    private string LoadHtml()
    {
        using(var streamReader = new StreamReader(typeof(MileageGridBuilder).Assembly.GetManifestResourceStream(
            typeof(MileageGridBuilder).Assembly.GetManifestResourceNames().Single(n => n.EndsWith("MileageGrid.html")))))
        {
            return streamReader.ReadToEnd();
        }
    }

    public Control BuildMileageGrid(cJourneyStep[] journeySteps, cSubcat subcat, HttpRequest request, int accountid, int expenseid, int employeeId, MileageUOM uom, int[] homeAddressIdsToReplace, string homeAddressKeyword, string officeLocationIdentifier)
    {
        var postedJourneySteps = GetJourneyStepsFromPostback(request, accountid, expenseid, subcat.subcatid);
        if (postedJourneySteps.Any()) journeySteps = postedJourneySteps; //this is in case the postback is rejected...we want to keep the values the user entered on the page
        cMisc misc = new cMisc(accountid);
        var fromField = misc.GetGeneralFieldByCode("from");
        var toField = misc.GetGeneralFieldByCode("to");
        var ph = new PlaceHolder();
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var root = doc.DocumentNode;
        var table = root.SelectSingleNode("table");
        var tbody = table.SelectSingleNode("tbody");
        var templateRow = tbody.SelectSingleNode("tr[@class='template']");

        HtmlNode fromHeaderDiv = table.SelectSingleNode(".//th[@data-field='from']/div");
        fromHeaderDiv.RemoveAllChildren();
        fromHeaderDiv.AppendChild(doc.CreateTextNode(fromField.description));

        HtmlNode toHeaderDiv = table.SelectSingleNode(".//th[@data-field='to']/div");
        toHeaderDiv.RemoveAllChildren();
        toHeaderDiv.AppendChild(doc.CreateTextNode(toField.description));

        HtmlNode rowToInsertAfter = null;
        for (int i = 0; i < journeySteps.Length; i++)
        {
            if (journeySteps[i].startlocation != null && journeySteps[i].endlocation != null)
            {


                int mileageRowNum1Based = i + 1;
                string startLocationIdentifier = journeySteps[i].startlocation.Identifier.ToString("0");
                string startLocationText = journeySteps[i].startlocation.FriendlyName;
                if (homeAddressIdsToReplace.Contains(journeySteps[i].startlocation.Identifier))
                {                    
                    startLocationText = homeAddressKeyword;
                }

                string endLocationIdentifier = journeySteps[i].endlocation.Identifier.ToString("0");
                string endLocationText = journeySteps[i].endlocation.FriendlyName;
                if (homeAddressIdsToReplace.Contains(journeySteps[i].endlocation.Identifier))
                {
                    endLocationText = homeAddressKeyword;
                }
                string recMiles = journeySteps[i].recmiles.ToString("0.00");
                string actualMiles = journeySteps[i].NumActualMiles.ToString("0.00");
                string passengersText = string.Join(",", journeySteps[i].passengers.Select(p => p.ToString()));
                int numPassengers = journeySteps[i].numpassengers;
                var heavyBulkyEquipment = journeySteps[i].heavyBulkyEquipment && subcat.allowHeavyBulkyMileage;
                var newRow = CreateRow(templateRow, doc, mileageRowNum1Based, startLocationIdentifier, startLocationText, endLocationIdentifier, endLocationText, recMiles, actualMiles, passengersText, numPassengers, heavyBulkyEquipment, subcat, uom, officeLocationIdentifier);

                tbody.InsertAfter(newRow, rowToInsertAfter);
                rowToInsertAfter = newRow;
            }
        }
        if (!subcat.allowHeavyBulkyMileage)
        {
            foreach (var heavyBulkyTdOrTh in table.SelectNodes(".//td[@class='heavybulky'] | .//th[@class='heavybulky']"))
            {
                heavyBulkyTdOrTh.Remove();
            }
        }

        if (!subcat.passengernamesapp) //displaying number of passengers, and not names
        {
            foreach (var passengerNamesTd in table.SelectNodes(".//td[@class='passengernames']"))
            {
                passengerNamesTd.Remove();
            }
        }
        if(!subcat.nopassengersapp || (subcat.passengernamesapp))
            //we never show both number and names (otherwise there would need to be more than th in the header row), but we might show neither.
        {
            foreach(var tr in table.SelectNodes(".//tbody/tr"))
            {
                foreach (var numPassengersTd in tr.SelectNodes(".//td[@class='numpassengers']"))
                {
                    if (!(subcat.nopassengersapp || subcat.passengernamesapp))
                    {
                        //if we're not showing either, we need to salvage the input fields to keep the postback in sync
                        HtmlNodeCollection passengersInputNodes = numPassengersTd.SelectNodes("input");
                        foreach (var passengersInputNode in passengersInputNodes)
            {
                            passengersInputNode.Attributes["type"].Value = "hidden";
                        }
                        tr.SelectSingleNode("td").AppendChildren(passengersInputNodes);
                    }
                numPassengersTd.Remove();
            }
        }
        }

        if (!(subcat.nopassengersapp || subcat.passengernamesapp))
            //if we're showing neither, we need to remove the 'Passengers' title out of the header row.
        {
            foreach (var node in table.SelectNodes(".//th[@class='passengersheader']"))
            {
                node.Remove();
            }
        }

        foreach (var subcatidNode in root.SelectNodes(".//input[@data-field='subcatid']")) //including the template row
        {
            subcatidNode.SetAttributeValue("value", subcat.subcatid.ToString());
        }

        // add a class to auto-display the contextual help if the employee hasn't previously closed it
        var contextualHelp = new EmployeeContextualHelp(accountid, employeeId);
        
        // this value is hardcoded until the rest of the contextual help feature is implemented
        if (contextualHelp.Show(1))
        {
            var contextualHelpElement = root.SelectSingleNode("div[contains(@class, 'contextualhelp')]");
            contextualHelpElement.SetAttributeValue("class", contextualHelpElement.GetAttributeValue("class", string.Empty) + " autoshow");
        }

        ph.Controls.Add(new Literal{Text = root.OuterHtml });
        return ph;
    }

    private static HtmlNode CreateRow(HtmlNode templateRow, HtmlDocument doc, int mileageRowNum1Based, string startLocationIdentifier, string startLocationText, string endLocationIdentifier, string endLocationText, string recMiles, string actualMiles, string passengersText, int numPassengers, bool heavyBulkyEquipment, cSubcat subcat, MileageUOM uom, string officeLocationIdentifier)
    {
        var newRow = templateRow.CloneNode(true);
        HtmlNode mileageRowNumSpan = newRow.SelectSingleNode("td/span[@data-field='mileagerownum']");
        mileageRowNumSpan.AppendChild(doc.CreateTextNode(mileageRowNum1Based.ToString("0")));
        mileageRowNumSpan.SetAttributeValue("style", mileageRowNumSpan.GetAttributeValue("style", "").Replace("display: none", ""));

        newRow.SelectSingleNode("td/input[@data-field='from_id']").SetAttributeValue("value", startLocationIdentifier);

        newRow.SelectSingleNode("td/input[@data-field='from_search']").SetAttributeValue("value", startLocationText);

        if (startLocationIdentifier == officeLocationIdentifier)
        {
            newRow.SelectSingleNode("td/input[@data-field='from_search']").Attributes.Add("data-office", "true");
        }

        newRow.SelectSingleNode("td/input[@data-field='to_id']").SetAttributeValue("value", endLocationIdentifier);

        newRow.SelectSingleNode("td/input[@data-field='to_search']").SetAttributeValue("value", endLocationText);

        if (endLocationIdentifier == officeLocationIdentifier)
        {
            newRow.SelectSingleNode("td/input[@data-field='to_search']").Attributes.Add("data-office", "true");
        }

        newRow.SelectSingleNode("td/input[@data-field='recommendeddistance']").SetAttributeValue("value", recMiles);
        foreach (var numPassengersNode in newRow.SelectNodes("td//input[@name='mileagegridtable_numpassengers']"))
        {
            numPassengersNode.SetAttributeValue("value", numPassengers.ToString());
        }
        newRow.SelectSingleNode("td/input[@data-field='uom']").SetAttributeValue("value", uom.ToString().ToLower());
        newRow.SelectSingleNode("td/input[@data-field='userentereddistance']").SetAttributeValue("value", actualMiles);

        foreach (var passengerInput in newRow.SelectNodes("td//input[@data-field='passengers']"))
        {
            //there's multiple ones - a hidden, which has a name and provides the value to ASP.NET,
            //and the search box that goes in the dialog.
            passengerInput.SetAttributeValue("value", passengersText);
        }

        newRow.SelectSingleNode("td/input[@data-field='heavybulkyequipment'][@type='hidden']").SetAttributeValue("value", heavyBulkyEquipment.ToString());
        if (heavyBulkyEquipment)
        {
            newRow.SelectSingleNode("td/input[@data-field='heavybulkyequipment'][@type='checkbox']").SetAttributeValue("checked", "checked");
        }

        // show the warning triangle if this journey step has an address from mobile that could not be matched (signified by -100)
        if (startLocationIdentifier == "-100" || endLocationIdentifier == "-100")
        {
            if (startLocationIdentifier == "-100")
            {
                newRow.SelectSingleNode("td/input[@data-field='from_search']").SetAttributeValue("style", "font-weight: bold");
            }

            if (endLocationIdentifier == "-100")
            {
                newRow.SelectSingleNode("td/input[@data-field='to_search']").SetAttributeValue("style", "font-weight: bold");
            }

            newRow.SelectSingleNode("td/input[@data-field='userentereddistance']").SetAttributeValue("value", string.Empty);
            newRow.SelectSingleNode("td/input[@data-field='recommendeddistance']").SetAttributeValue("value", string.Empty);
            newRow.SelectSingleNode("td/span[@data-field='nodistancewarningicon']").SetAttributeValue("style", "visibility:visible");
            newRow.SelectSingleNode("td/div[@data-field='nodistancewarningmessage']").InnerHtml = "The highlighted address could not be matched automatically, please try searching manually. If you are still unable to find the address please contact your administrator.";
        }

        newRow.SetAttributeValue("class", string.Empty); //remove 'template'

        return newRow;
    }

    public static cJourneyStep[] GetJourneyStepsFromPostback(HttpRequest request, int accountId, int expenseId, int subcatid)
    {
        List<cJourneyStep> journeySteps = new List<cJourneyStep>();
        string[] journeyStepProperties = new[]
            {
                "from_address_id",
                "to_address_id",
                "from_address_text",
                "to_address_text",
                "recommendeddistance",
                "userentereddistance",
                "passengers",
                "heavybulkyequipment",
                "subcatid",
                "numpassengers",
                "uom"
            };
        //create a property lookup for each journey step:
        const string formPrefix = "mileagegridtable_";
        var journeyStepValues = (request.Form.GetValues(formPrefix + journeyStepProperties[0]) ?? new string[0])
                                       .Select(v => new Dictionary<string, string> {{journeyStepProperties[0], v}}).ToArray();
        //add in the rest of the properties
        foreach (var journeyStepProperty in journeyStepProperties.Skip(1))
        {
            var values = request.Form.GetValues(formPrefix + journeyStepProperty) ??  new string[0];
            for(int i = 0; i < Math.Min(values.Length, journeyStepValues.Length); i++)
            {
                journeyStepValues[i][journeyStepProperty] = values[i];
            }
        }
        //... so now journeyStepValues is an array of dictionaries with journeyStepProperties as keys.

        byte stepnumber = 0;
        for (int i = 0; i < journeyStepValues.Length; i++)
        {
            var journeyStepValue = journeyStepValues[i];
            int fromAddressId = 0, toAddressId = 0;
            decimal nummiles = 0, recmiles = 0;
            int formsubcatid;
            if (int.TryParse(journeyStepValue["from_address_id"], out fromAddressId) && //this will be false for the template (or any un-filled in row)
                int.TryParse(journeyStepValue["to_address_id"], out toAddressId) &&
                (string.IsNullOrEmpty(journeyStepValue["userentereddistance"]) ||
                 decimal.TryParse("0" + journeyStepValue["userentereddistance"], out nummiles)) &&
                decimal.TryParse("0" + journeyStepValue["recommendeddistance"], out recmiles) &&
                int.TryParse(journeyStepValue["subcatid"], out formsubcatid) && subcatid == formsubcatid)
            {
                Passenger[] passengers = new Passenger[0];
                string passengersString = "";
                int numpassengers = 0;
                if (journeyStepValue.TryGetValue("passengers", out passengersString))
                {
                    passengers = Passenger.ParsePassengersString(passengersString).ToArray();
                    numpassengers = passengers.Length;
                }


                string numPassengersString = "";

                if(journeyStepValue.TryGetValue("numpassengers", out numPassengersString))
                {
                    if (!int.TryParse(numPassengersString, out numpassengers))
                        //try to use the submitted form value if possible,
                {
                        numpassengers = passengers.Length;
                        ///..but otherwise the number of passengers is the number that are named
                        /// (execution should always get here if passenger names is on as numpassengers will be blank)
                    }
                }

                

                string heavyBulkyEquipmentString;
                bool heavyBulkyEquipment = false;
                if (journeyStepValue.TryGetValue("heavybulkyequipment", out heavyBulkyEquipmentString))
                {
                    bool.TryParse(journeyStepValue["heavybulkyequipment"], out heavyBulkyEquipment);
                }

                string journeyStepUomString;
                journeyStepValue.TryGetValue("uom", out journeyStepUomString);
                MileageUOM? journeyStepUom = Enum.GetValues(typeof(MileageUOM))
                    .Cast<MileageUOM?>().FirstOrDefault(s =>
                        !string.IsNullOrEmpty(journeyStepUomString) && journeyStepUomString.StartsWith(s.ToString(), StringComparison.InvariantCultureIgnoreCase));
                //(used StartsWith as it might have 's' on the end, e.g. miles, kms)
                if (Equals(MileageUOM.KM, journeyStepUom))
                {
                    const decimal kmPerMile = 1.609344m;
                    nummiles /= kmPerMile;
                    recmiles /= kmPerMile;
                }


                Address fromAddress = null;
                if (fromAddressId > 0)
                {
                    fromAddress = Address.Get(accountId, fromAddressId);
                }
                else
                {
                    fromAddress = new Address();

                    string addressNameString;
                    journeyStepValue.TryGetValue("from_address_text", out addressNameString);
                    fromAddress.Identifier = -100;
                    fromAddress.Line1 = addressNameString;
                }

                Address toAddress = null;
                if (toAddressId > 0)
                {
                    toAddress = Address.Get(accountId, toAddressId);
                }
                else
                {
                    toAddress = new Address();

                    string addressNameString;
                    journeyStepValue.TryGetValue("to_address_text", out addressNameString);
                    toAddress.Identifier = -100;
                    toAddress.Line1 = addressNameString;
                }

                journeySteps.Add(new cJourneyStep(expenseId, fromAddress, toAddress, nummiles, recmiles, (byte)numpassengers,
                                                  stepnumber++, nummiles,
                                                  heavyBulkyEquipment) {passengers = passengers});
            }
        }
        return journeySteps.ToArray();
    }
}
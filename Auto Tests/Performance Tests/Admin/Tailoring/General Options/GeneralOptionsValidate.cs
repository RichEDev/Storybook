
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class GeneralOptionsValidate : WebTest
    {

        public GeneralOptionsValidate(string changeItem, string changeValue, string itemType)
        {
            this.Context.Add("WebServer1", AutoTools.ServerToUse());
            this.PreAuthenticate = true;
            this.Context.Add("changeItem", changeItem);
            this.Context.Add("changeValue", changeValue);
            this.Context.Add("itemType", itemType);
        }

        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            // Initialize validation rules that apply to all requests in the WebTest
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.Low))
            {
                ValidateResponseUrl validationRule1 = new ValidateResponseUrl();
                this.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule1.Validate);
            }

            // Login required?
            //foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

            // Validate that the changes have been saved properly
            WebTestRequest requestValidate = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/accountOptions.aspx");

            switch (this.Context["itemType"].ToString())
            {
                case "chkon":
                    // Enter validation for checkboxes that should be ticked

                    if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
                    {
                        ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                        validationRule2.FindText = "name=\"ctl00$contentmain$" + this.Context["changeItem"].ToString() + "\"" +
                            " checked=\"checked\"";
                        validationRule2.IgnoreCase = false;
                        validationRule2.UseRegularExpression = false;
                        validationRule2.PassIfTextFound = true;
                        requestValidate.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
                    }
                    break;

                case "chkoff":
                    // Enter validation for checkboxes that should be UN-ticked

                    if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
                    {
                        ValidationRuleFindText validationRule2 = new ValidationRuleFindText();

                        // The following options have slightly different validation text 
                        if (this.Context["changeItem"].ToString().Contains("chkAllowUsersToAddCars") == true)
                        {
                            validationRule2.FindText = "name=\"ctl00$contentmain$" + this.Context["changeItem"].ToString() + "\" onclick=\"EnableUsersAddCar();\" />";
                        }
                        else if (this.Context["changeItem"].ToString().Contains("chkActivateCarOnUserAdd") == true)
                        {
                            validationRule2.FindText = "name=\"ctl00$contentmain$" + this.Context["changeItem"].ToString() + "\" onclick=\"activateCarsOnAdd();\" />";
                        }
                        else
                        {
                            validationRule2.FindText = "name=\"ctl00$contentmain$" + this.Context["changeItem"].ToString() + "\" />";
                        }
                        validationRule2.IgnoreCase = false;
                        validationRule2.UseRegularExpression = false;
                        validationRule2.PassIfTextFound = true;
                        requestValidate.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
                    }
                    break;

                case "list":
                    // Enter validation for drop down lists

                    if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
                    {
                        ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                        validationRule2.FindText = "option selected=\"selected\" value=\"" + this.Context["changeValue"].ToString() + "\">";
                        validationRule2.IgnoreCase = false;
                        validationRule2.UseRegularExpression = false;
                        validationRule2.PassIfTextFound = true;
                        requestValidate.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
                    }
                    break;

                case "txt":
                    // Enter validation for txt values

                    if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
                    {
                        // Multiple-line txt fields need different validation
                        if (this.Context["changeItem"].ToString().Contains("msg"))
                        {
                            ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                            validationRule2.FindText = this.Context["changeItem"].ToString().Substring(35) +
                                "\" style=\"width:300px;\">" + this.Context["changeValue"].ToString() + "<";
                            validationRule2.IgnoreCase = false;
                            validationRule2.UseRegularExpression = false;
                            validationRule2.PassIfTextFound = true;
                            requestValidate.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
                        }
                        else
                        {
                            ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                            validationRule2.FindText = "ctl00$contentmain$" + this.Context["changeItem"].ToString() +
                                "\" type=\"text\" value=\"" + this.Context["changeValue"].ToString() + "\"";
                            validationRule2.IgnoreCase = false;
                            validationRule2.UseRegularExpression = false;
                            validationRule2.PassIfTextFound = true;
                            requestValidate.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
                        }
                    }
                    break;

                case "rbtn":
                    // Enter validation for radio buttons

                    string selectedValue = "";
                    if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
                    {
                        ValidationRuleFindText validationRule2 = new ValidationRuleFindText();

                        if (this.Context["changeItem"].ToString() == "tabsGeneralOptions$tabGeneral$odometerentry")
                        {
                            if (this.Context["changeValue"].ToString().Length == 1)
                            {
                                selectedValue = "optodosubmit";
                            }
                            else
                            {
                                selectedValue = "optodologin";
                            }
                        }
                        else if (this.Context["changeItem"].ToString() == "tabsGeneralOptions$tabGeneral$locations")
                        {
                            if (this.Context["changeValue"].ToString().Length == 1)
                            {
                                selectedValue = "optlocationsearch";
                            }
                            else
                            {
                                selectedValue = "optlocationdd";
                            }
                        }
                        else if (this.Context["changeItem"].ToString() == "source")
                        {
                            if (this.Context["changeValue"].ToString().Length == 1)
                            {
                                selectedValue = "optclaimant";
                            }
                            else
                            {
                                selectedValue = "optserver";
                            }
                        }

                        validationRule2.FindText = "value=\"" + selectedValue + "\" checked=\"checked\"";
                        requestValidate.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
                    }

                    break;

                default:

                    break;
            }

            // Clear the stored values before the next test is run
            this.Context.Clear();

            yield return requestValidate;
            requestValidate = null;                       
        }
    }
}
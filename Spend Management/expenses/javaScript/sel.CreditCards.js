/// <summary>
/// Attachment type Methods
/// </summary>    
(function ()
{
    var scriptName = "creditCards";
    function execute()
    {
        SEL.registerNamespace("SEL.CreditCards");
        SEL.CreditCards =
        {

            /// <summary>
            /// Card Company Object
            /// </summary>
            CardCompany: function (nID, sCompanyName, sCompanyNumber, bUsedForImport)
            {
                this.cardCompanyID = nID;
                this.companyName = sCompanyName;
                this.companyNumber = sCompanyNumber;
                this.usedForImport = bUsedForImport;
            },

            /// <summary>
            /// Save the credit card companies to the database
            /// </summary>
            SaveCardCompanies: function ()
            {
                //On the card companies step if not null
                if (document.getElementById(cardCompanyTableID) !== null)
                {
                    var Inputs = document.getElementById(cardCompanyTableID).getElementsByTagName("input");
                    var cardCompanies = new Array();
                    var tempVals;
                    var cardCompanyObject;

                    for (var i = 0; i < Inputs.length; i++)
                    {
                        if (Inputs[i].type === "checkbox")
                        {
                            tempVals = Inputs[i].value.split(',');

                            cardCompanyObject = new this.CardCompany(parseInt(tempVals[0]), tempVals[1], tempVals[2], Inputs[i].checked);
                            cardCompanies.push(cardCompanyObject);
                        }
                    }

                    if (cardCompanies.length > 0)
                    {
                        Spend_Management.svcCreditCards.SaveCardCompanies(cardCompanies, null, this.OnError);
                    }
                }
            },

            /// <summary>
            /// Catch any error and show an error message to users
            /// </summary>
            OnError: function (error)
            {
                if (error["_message"] != null)
                {
                    SEL.MasterPopup.ShowMasterPopup(error["_message"], "Credit Card Error");
                }
                else
                {
                    SEL.MasterPopup.ShowMasterPopup(error, "Credit Card Error");
                }
                return;
            }

        };
    }

    if (window.Sys && Sys.loader)
    {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else
    {
        execute();
    }
})();


namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddEveryExpense : WebTest
    {

        public AddEveryExpense()
        {
            this.Context.Add("WebServer1", AutoTools.ServerToUse());
            this.PreAuthenticate = true;
        }

        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            // Initialize validation rules that apply to all requests in the WebTest
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.Low))
            {
                ValidateResponseUrl validationRule1 = new ValidateResponseUrl();
                this.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule1.Validate);
            }


            // Delete any claims that currently exist
            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.DeleteClaim(), false)) { yield return r; }

            // Add a new claim
            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddFirstClaim(), false)) { yield return r; }

            string subcatID = "";
            string subcatName = "";

            // Connect to the database and use each Auto expense item
            
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT subcatid, subcat FROM subcats WHERE subcat LIKE '__Auto%'");
            while (reader.Read())
            {
                subcatID = reader.GetValue(0).ToString();
                subcatName = reader.GetValue(1).ToString().Remove(0,7);
                subcatName = subcatName.Remove(subcatName.Length - 5, 5);

                // Set 'My Claimable Items'
                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.SelectSpecificItem(subcatID), false)) { yield return r; }

                switch (subcatName)
                {
                    case "Hotel":

                        //foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddHotelExpense(), false)) { yield return r; }                      
                        break;

                    case "Fixed Allowance":

                        foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddFixedAllowanceExpense(), false)) { yield return r; }
                        break;

                    case "Daily Allowance":

                        foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddDailyAllowanceExpense(), false)) { yield return r; }
                        break;

                    case "Meal Split":

                        foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddMealSplitExpense(), false)) { yield return r; }
                        break;

                    case "Meal":

                        foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddMealExpense(), false)) { yield return r; }
                        break;

                    case "Standard":

                        foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddStandardExpense(), false)) { yield return r; }
                        break;

                    case "Standard All":

                        foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddStandardAllExpense(), false)) { yield return r; }
                        break;

                    case "Mileage":

                        foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddMileageExpense(), false)) { yield return r; }
                        break;

                    default:

                        break;
                }
            }
            reader.Close();
        }
    }
}

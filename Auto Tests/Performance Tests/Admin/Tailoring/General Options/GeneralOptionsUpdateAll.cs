

// !! This test will reset all settings in general options !!
// !! Due to the size of the test, it should only be used during a build or as a load test !!


namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class GeneralOptionsUpdateAll : WebTest
    {

        public GeneralOptionsUpdateAll()
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

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }            

            // Check that all checkboxes can be ticked
            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.GeneralOptionsSelectChk(), false)) { yield return r; } 

            // Check that all checkboxes can be UN-ticked
            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.GeneralOptionsUnselectChk(), false)) { yield return r; }

            // Check that all lists can be changed
            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.GeneralOptionsChangeList(), false)) { yield return r; }

            // Check that all text fields can be changed
            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.GeneralOptionsChangeTxt(), false)) { yield return r; }

            // Check that all radio buttons can be changed
            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.GeneralOptionsChangeRbtn(), false)) { yield return r; }         
        }
    }
}
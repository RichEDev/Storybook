using System;
using Microsoft.VisualStudio.TestTools.WebTesting;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;

namespace SpendManagement.AutomatedTests.Runtime.Plugins
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewStateHandler : WebTestPlugin
    {
        /// <summary>
        /// Represents the method that will handle the event associated with the start
        /// of an HTTP request.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A PreRequestEventArgs that contains the event data.</param>
        public override void PreRequest(object sender, PreRequestEventArgs e)
        {
            if (e.Request.Method.ToUpper() == "GET")
            {
                ExtractFormField viewStateExtract = new ExtractFormField();
                viewStateExtract.Name = "__VIEWSTATE";
                viewStateExtract.HtmlDecode = true;
                viewStateExtract.Required = true;
                viewStateExtract.ContextParameterName = "ViewState";

                ExtractFormField eventTargetExtract = new ExtractFormField();
                eventTargetExtract.Name = "__EVENTTARGET";
                eventTargetExtract.HtmlDecode = true;
                eventTargetExtract.Required = false;
                eventTargetExtract.ContextParameterName = "EventTarget";

                ExtractFormField eventArgumentExtract = new ExtractFormField();
                eventArgumentExtract.Name = "__EVENTARGUMENT";
                eventArgumentExtract.HtmlDecode = true;
                eventArgumentExtract.Required = false;
                eventArgumentExtract.ContextParameterName = "EventArgument";

                e.Request.ExtractValues += new EventHandler<ExtractionEventArgs>(viewStateExtract.Extract);
                e.Request.ExtractValues += new EventHandler<ExtractionEventArgs>(eventTargetExtract.Extract);
                e.Request.ExtractValues += new EventHandler<ExtractionEventArgs>(eventArgumentExtract.Extract);
            }
            else if (e.Request.Method.ToUpper() == "POST")
            {
                if (e.Request.Body.GetType() == typeof(FormPostHttpBody))
                {
                    FormPostHttpBody body = (FormPostHttpBody)e.Request.Body;

                    body.FormPostParameters.Add("__VIEWSTATE", e.WebTest.Context["ViewState"].ToString());
                    body.FormPostParameters.Add("__EVENTTARGET", e.WebTest.Context["EventTarget"].ToString());
                    body.FormPostParameters.Add("__EVENTARGUMENT", e.WebTest.Context["EventArgument"].ToString());
                }
            }
        }
    }
}

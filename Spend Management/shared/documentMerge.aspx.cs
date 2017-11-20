namespace Spend_Management
{
    using System;
    using System.Configuration;
    using System.Threading;
    using System.Web.UI;

    using SpendManagementLibrary.DocumentMerge;

    using Syncfusion.DocIO;
    using Syncfusion.DocIO.DLS;

    public partial class documentMerge : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["mprj"] != null)
            {
                var currentUser = cMisc.GetCurrentUser();
                string closeScript;
                int mergeprojectid = int.Parse(Request.QueryString["mprj"]);
                Guid mergeRequestNumber;
                if (Request.QueryString["mrn"] != null)
                {
                    mergeRequestNumber = Guid.Parse(Request.QueryString["mrn"]);
                }
                else
                {
                    mergeRequestNumber = Guid.NewGuid();
                    Session["currentMergeRequestNumber"] = mergeRequestNumber;
                }

                litMergeProgress.Text = "Please wait, collecting data...";

                if (Request.QueryString["mc"] != null)
                {
                    for (var attempt = 1; attempt <= int.Parse(ConfigurationManager.AppSettings["DocMergeRetryAttempts"]); attempt++)
                    {
                        try
                        {
                            var mergeStatus = TorchMergeState.Get(mergeprojectid, mergeRequestNumber, currentUser);
                            string filename = "Torch_MergeDoc_" + DateTime.Now.ToShortDateString().Replace("/", "") + DateTime.Now.ToShortTimeString().Replace(":", "") + (mergeStatus.OutputDocType == TorchExportDocumentType.MS_Word_DOCX ? ".docx" : ".doc");
                            WordDocument document = null;
                            try
                            {
                                document = new WordDocument(mergeStatus.DocumentPath)
                                {
                                    ProtectionType = ProtectionType.NoProtection,
                                    ThrowExceptionsForUnsupportedElements = false
                                };

                                // Note: This method throws a ThreadAbortException on success
                                document.Save(filename, (mergeStatus.OutputDocType == TorchExportDocumentType.MS_Word_DOCX ? FormatType.Docx : FormatType.Doc), Response, HttpContentDisposition.Attachment);
                            }
                            finally
                            {
                                if (document != null)
                                {
                                    document.Close();
                                }
                            }
                        }
                        catch (ThreadAbortException)
                        {
                            TorchMergeState.Cleanup(currentUser);
                            throw;
                        }
                        catch
                        {
                            Thread.Sleep(int.Parse(ConfigurationManager.AppSettings["DocMergeRetrySleepTime"]));
                        }
                    }

                    closeScript = "alert('A problem occurred during the Torch process and the output document could not be opened.'); window.close();";
                    this.litMergeProgress.Text = "A problem occurred during the Torch process and the output document could not be opened.<br />" +
                        "Please check your document configuration or try Perform Merge and add to Torch History.";

                }
                else
                {
                    closeScript = string.Format("SEL.DocMerge.InitMerge({0}, {1});", mergeprojectid, mergeRequestNumber);
                }

                this.ClientScript.RegisterStartupScript(this.GetType(), "closeScript", closeScript, true);

            }
        }
    }
}

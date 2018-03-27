using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SpendManagementLibrary;
using SpendManagementLibrary.GreenLight;

using Syncfusion.DocIO.DLS;
using SpendManagementLibrary.ESRTransferServiceClasses;
using System.Configuration;
using System.Runtime.Remoting.Channels;
using System.Collections;
using System.Runtime.Remoting.Channels.Http;
using System.Web.SessionState;

namespace Spend_Management
{
    using System.Globalization;

    using Spend_Management.shared.code;
    using SpendManagementLibrary.HelpAndSupport;
    using SpendManagementLibrary.Enumerators;

    /// <summary>
    /// getDocument: Handler class used for downloading and saving documents
    /// </summary>
    public class getDocument : IHttpHandler, IReadOnlySessionState
    {
        /// <summary>
        /// 
        /// </summary>
        bool IHttpHandler.IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the IHttpHandler interface.
        /// </summary>
        /// <param name="context">
        /// An HttpContext object that provides references to the server objects used to service HTTP requests.
        /// </param>
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            int? delegateId = null;
            if (curUser.isDelegate)
            {
                delegateId = curUser.Delegate.EmployeeID;
            }

            DocumentType docType = DocumentType.Unspecified;

            if (context.Request.QueryString["docType"] != null)
            {
                docType = (DocumentType)Convert.ToByte(context.Request.QueryString["docType"]);
            }

            switch (docType)
            {
                case DocumentType.ESROutbound:

                    if (context.Request.QueryString["dataID"] != null)
                    {
                        int DataID = Convert.ToInt32(context.Request.QueryString["dataID"]);

                        BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
                        //BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
                        //serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

                        int chanCount = 0;
                        IDictionary props = new Hashtable();
                        props["port"] = 0;
                        props["typeFilterLevel"] = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                        HttpChannel chan = new HttpChannel(props, clientProvider, null);

                        foreach (IChannel c in ChannelServices.RegisteredChannels)
                        {
                            if (c.ChannelName == chan.ChannelName)
                            {
                                chanCount++;
                            }
                        }

                        if (chanCount == 0)
                        {
                            ChannelServices.RegisterChannel(chan, false);
                        }

                        IESRTransfer clsESR = (IESRTransfer)Activator.GetObject(typeof(IESRTransfer), ConfigurationManager.AppSettings["ESRTransferServicePath"] + "/ESRTransfer.rem");

                        cESRFileInfo[] lstData = clsESR.getOutboundData(DataID);
                        byte[] data = lstData[0].FileData;

                        context.Response.Clear();
                        context.Response.AddHeader("Content-Length", data.Length.ToString());
                        context.Response.ContentType = "text/plain";
                        //context.Response.CacheControl = "no-cache";
                        context.Response.AddHeader("Content-Disposition", "attachment; filename=" + lstData[0].fileName); //+ Use file name returned from web service(" ", "_") + ";");
                        context.Response.OutputStream.Write(data, 0, data.Length);
                        context.Response.End();
                    }

                    break;

                case DocumentType.Receipts:

                    if (context.Request.QueryString["receiptId"] != null)
                    {
                        var receiptId = Convert.ToInt32(context.Request.QueryString["receiptId"]);
                        var receipts = new SpendManagementLibrary.Expedite.Receipts(curUser.AccountID, curUser.EmployeeID);
                        var receipt = receipts.GetById(receiptId);
                        if (receipt != null)
                        {
                            var expenseId = receipt.OwnershipInfo.ClaimLines;
                            if (expenseId.Count == 0)
                            {
                                this.DownloadReceipt(receiptId, curUser, ref context);
                            }

                            var claims = new cClaims(curUser.AccountID);
                            var expenseitem = claims.getExpenseItemById(expenseId[0]);

                            if (expenseitem != null)
                            {
                                var claim = claims.getClaimById(expenseitem.claimid);
                                if (claim == null)
                                {
                                    context.Response.Write(ErrorHandlerWeb.MissingRecordUrl);
                                }

                                ClaimToAccessStatus canAccessClaim = claims.CheckClaimAndOwnership(claim, curUser, true);
                                switch (canAccessClaim)
                                {
                                    case ClaimToAccessStatus.ClaimNotFound:
                                        context.Response.Write(ErrorHandlerWeb.MissingRecordUrl);
                                        break;
                                    case ClaimToAccessStatus.InSufficientAccess:
                                        context.Response.Write("Restricted");
                                        break;
                                    case ClaimToAccessStatus.Success:
                                        this.DownloadReceipt(int.Parse(context.Request.QueryString["receiptId"]), curUser, ref context);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                        }
                    }

                    break;

                default:
                    if (context.Request.QueryString["receiptId"] != null)
                    {
                        DownloadReceipt(int.Parse(context.Request.QueryString["receiptId"]), curUser, ref context);
                        return;
                    }

                    if (context.Request.QueryString["supportTicketSalesForceId"] != null && context.Request.QueryString["supportTicketSalesForceAttachmentId"] != null)
                    {
                        SupportTicketSalesForceAttachment.Download(context.Request.QueryString["supportTicketSalesForceAttachmentId"], context.Request.QueryString["supportTicketSalesForceId"], curUser, ref context);
                        return;
                    }

                    if (context.Request.QueryString["supportTicketId"] != null && context.Request.QueryString["supportTicketAttachmentId"] != null)
                    {
                        SupportTicketAttachment.Download(int.Parse(context.Request.QueryString["supportTicketAttachmentId"]), int.Parse(context.Request.QueryString["supportTicketId"]), curUser, ref context);
                        return;
                    }

                    if (context.Request.QueryString["mobileID"] != null)
                    {
                        showMobileReceipt(int.Parse(context.Request.QueryString["mobileID"]), curUser, ref context);
                        return;
                    }

                    if (context.Request.QueryString["attid"] != null)
                    {
                        int attid = int.Parse(context.Request.QueryString["attid"]);
                        Guid tableid = new Guid(context.Request.QueryString["tableid"]);

                        cAttachments atts = new cAttachments(curUser.AccountID, curUser.EmployeeID, curUser.CurrentSubAccountId, delegateId);
                        cAttachment att = atts.getAttachment(tableid, attid, curUser, new cAuditLog(curUser.AccountID, curUser.EmployeeID));

                        cGlobalMimeTypes clsGlobMimeTypes = new cGlobalMimeTypes(curUser.AccountID);

                        cGlobalMimeType gMime = clsGlobMimeTypes.getMimeTypeById(att.MimeType.GlobalMimeID);

                        context.Response.Clear();
                        context.Response.AddHeader("Content-Length", att.AttachmentData.Length.ToString());
                        context.Response.ContentType = gMime.MimeHeader;
                        //context.Response.CacheControl = "no-cache";
                        context.Response.AddHeader("Content-Disposition", "attachment; filename=" + att.FileName.Replace(" ", "_") + ";");
                        context.Response.OutputStream.Write(att.AttachmentData, 0, att.AttachmentData.Length);
                        context.Response.End();

                        return;
                    }

                    if (context.Request.QueryString["fileID"] != null)
                    {
                        var customEntities = new cCustomEntities(curUser);
                        cCustomEntity currentEntity = null;
                        cCustomEntityView currentView = null;
                        if (context.Request.QueryString["entityid"] != null
                            && context.Request.QueryString["viewid"] != null
                            && context.Request.QueryString["recordId"] != null)
                        {
                            var viewId = int.Parse(context.Request.QueryString["viewid"]);
                            var entityId = int.Parse(context.Request.QueryString["entityid"]);
                            var recordId = int.Parse(context.Request.QueryString["recordId"]);
                            currentEntity = customEntities.getEntityById(entityId);
                            currentView = currentEntity.getViewById(viewId);

                            if (!customEntities.IsTheDataAccessibleToTheUser(currentView, currentEntity, recordId))
                            {
                                this.DisplayInsufficientAccessMessage(context);
                                return;
                            }
                        }
                        else if (context.Request.QueryString["isLookUpDisplayField"] == null || bool.Parse(context.Request.QueryString["isLookUpDisplayField"]) != true)
                        {
                            this.DisplayInsufficientAccessMessage(context);
                            return;
                        }

                        string fileID = (context.Request.QueryString["fileID"]);

                        HtmlImageData fileData = customEntities.GetCustomEntityAttachmentData(curUser, new cAuditLog(curUser.AccountID, curUser.EmployeeID), fileID);

                        if (fileData != null)
                        {
                            context.Response.Clear();
                            context.Response.AddHeader("Content-Length", fileData.ImageData.Length.ToString());
                            context.Response.ContentType = fileData.FileType;
                            context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileData.FileName.Replace(" ", "_") + ";");
                            context.Response.OutputStream.Write(fileData.ImageData, 0, fileData.ImageData.Length);
                            context.Response.End();
                        }
                        return;
                    }

                    if (context.Request.QueryString["fileID"] != null)
                    {
                        string fileID = (context.Request.QueryString["fileID"]);

                        //get file details based on ID
                        cCustomEntities customEntities = new cCustomEntities();

                        HtmlImageData fileData = customEntities.GetCustomEntityAttachmentData(curUser, new cAuditLog(curUser.AccountID, curUser.EmployeeID), fileID);

                        if (fileData != null)
                        {
                            context.Response.Clear();
                            context.Response.AddHeader("Content-Length", fileData.ImageData.Length.ToString());
                            context.Response.ContentType = fileData.FileType;
                            context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileData.FileName.Replace(" ", "_") + ";");
                            context.Response.OutputStream.Write(fileData.ImageData, 0, fileData.ImageData.Length);
                            context.Response.End();
                        }
                        return;
                    }

                    var templates = new DocumentTemplate(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID);

                    string docId = context.Request.QueryString["id"];
                    if (docId == "" || docId == "undefined" || docId == null)
                    {
                        context.Response.Write("ERROR! An error occurred while attempting to retrieve the file");
                    }
                    else
                    {
                        cDocumentTemplate documentTemplate = templates.getTemplateById(int.Parse(docId));
                        if (documentTemplate != null)
                        {
                            byte[] docStream = templates.getDocument(int.Parse(docId));
                            context.Response.Clear();
                            context.Response.AddHeader("Content-Length", docStream.Length.ToString(CultureInfo.InvariantCulture));
                            context.Response.ContentType = documentTemplate.DocumentContentType;
                            context.Response.AddHeader("Content-Disposition", "attachment; filename=" + documentTemplate.DocumentFilename.Replace(" ", "_"));
                            context.Response.OutputStream.Write(docStream, 0, docStream.Length);
                            context.Response.End();
                        }
                    }

                    break;
            }

            return;
        }

        /// <summary>
        /// Display insufficient access message when a file.
        /// </summary>
        /// <param name="context">
        /// The current http context.
        /// </param>
        private void DisplayInsufficientAccessMessage(HttpContext context)
        {
            context.Response.Write("Insufficient access to retrieve the file");
        }

        protected void showMobileReceipt(int mobileItemID, CurrentUser curUser, ref HttpContext context)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));
            db.sqlexecute.Parameters.AddWithValue("@mobileItemID", mobileItemID);

            const string sql = "select receiptData from mobileExpenseItemReceipts where mobileID = @mobileItemID";
            byte[] receipt = db.getImageData(sql);

            if (receipt.Length > 0)
            {
                var receiptName = $"{curUser.AccountID}_{curUser.EmployeeID}_{mobileItemID}.png";
                AuditMobileReceiptView(receiptName, curUser);

                context.Response.Clear();
                context.Response.AddHeader("Content-Length", receipt.Length.ToString());
                context.Response.ContentType = "image/png"; // temporary until method of mime-header handling decided on
                //context.Response.CacheControl = "no-cache";
                context.Response.AddHeader("Content-Disposition", "attachment; filename=Receipt_" + receiptName);
                context.Response.OutputStream.Write(receipt, 0, receipt.Length);
                context.Response.End();
            }
        }

        /// <summary>
        /// Adds an audit entry to record the recipt being viewed by a delegate.
        /// </summary>
        /// <param name="receiptName">The name of the receipt.</param>
        /// <param name="currentUser">The <see cref="CurrentUser"/>.</param>
        private void AuditMobileReceiptView(string receiptName, CurrentUser currentUser)
        {       
            if (currentUser.isDelegate)
            {
                var record = "Receipt " + receiptName;
                var auditLog = new cAuditLog();
                auditLog.ViewRecord(SpendManagementElement.Receipts, record, currentUser);
            }
        }

        /// <summary>
        /// Streams a receipt file to the client
        /// </summary>
        /// <param name="receiptId">The receipt's Id</param>
        /// <param name="user">The user</param>
        /// <param name="context">The HTTP context</param>
        protected void DownloadReceipt(int receiptId, CurrentUser user, ref HttpContext context)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(user.AccountID));
            db.sqlexecute.Parameters.AddWithValue("@receiptId", receiptId);
            const string Sql = "SELECT CONVERT(varchar(50), ReceiptId) +'.'+ FileExtension FROM [receipts] WHERE receiptId = @receiptId;";

            var fileUrl = db.getStringValue(Sql);

            var filePath = $"~/receipts/{user.AccountID}/{fileUrl}";
            var filename = System.IO.Path.GetFileName(filePath);
            var extension = System.IO.Path.GetExtension(filename);

            var mimeTypes = new cGlobalMimeTypes(user.AccountID);
            var mimeType = mimeTypes.getMimeTypeByExtension(extension.Replace(".", string.Empty));

            context.Response.Clear();
            context.Response.ContentType = mimeType.MimeHeader;
            context.Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
            context.Response.ClearContent();
            context.Response.WriteFile(filePath);
            context.Response.End();
        }
    }
}

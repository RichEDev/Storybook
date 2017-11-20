// ReSharper disable once CheckNamespace
namespace expenses
{
    using System.Collections.Generic;
    using System.Web.Script.Services;
    using System.Web.Services;
    using Spend_Management;
    using System;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Script.Serialization;
    using SpendManagementLibrary.Enumerators.Expedite;
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.Expedite.DTO;
    using WebGrease.Css.Extensions;
    using Receipts = SpendManagementLibrary.Expedite.Receipts;
    using Syncfusion.Windows.Forms.PdfViewer;
    using System.Drawing;

    /// <summary>
    /// receipts web services
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class ReceiptsService : WebService
    {
        /// <summary>
        /// Fetches a list of receipts for a single expense item
        /// </summary>
        /// <param name="expenseId">The expense Id</param>
        /// <returns>A list of receipts</returns>
        [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<AttachedReceipt> GetReceiptsForExpenseItem(int expenseId)
        {
            var user = cMisc.GetCurrentUser();
            return new Receipts(user.AccountID, user.EmployeeID)
                                .GetByClaimLine(expenseId)
                                .Select(r => AttachedReceipt.FromReceipt(r, user.AccountID))
                                .ToList();
        }

        /// <summary>
        /// Fetches a list of receipts for a claim, which aren't associated with an expense item
        /// </summary>
        /// <param name="claimId">The claim Id</param>
        /// <returns>A list of receipts</returns>
        [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<AttachedReceipt> GetReceiptsForClaim(int claimId)
        {
            var user = cMisc.GetCurrentUser();
            return new Receipts(user.AccountID, user.EmployeeID)
                                .GetByClaim(claimId)
                                .Select(r => AttachedReceipt.FromReceipt(r, user.AccountID))
                                .ToList();
        }

        /// <summary>
        /// Fetches a Receipt, downloading it.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns>Json string representing the Id, url and whether the receipt is an image</returns>
        [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object GetReceiptFromCloudById(int id)
        {
            var user = cMisc.GetCurrentUser();
            var data = new Receipts(user.AccountID, user.EmployeeID);
            var receipt = data.GetById(id);
            var isImage = data.CheckIfReceiptIsImageAndOverwriteUrl(receipt);

            var output = new
            {
                id = receipt.ReceiptId,
                url = receipt.TemporaryUrl,
                isImage,
                icon = receipt.IconUrl
            };

            return new JavaScriptSerializer().Serialize(output);
        }

        /// <summary>
        /// Fetches a tree of objects, either from an account level all the way down to receipts.
        /// </summary>
        /// <param name="accountId">The account Id.</param>
        /// <returns>A list of receipts</returns>
        [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<EnvelopeManagementDTONode> GetEnvelopeSourceTree(int accountId)
        {
            var user = cMisc.GetCurrentUser();

            // check permissions
            if (!user.Account.ReceiptServiceEnabled || !user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.EnvelopeManagement, true))
            {
                throw new Exception("You don't have permission to perform this operation.");
            }

            var envelopeData = new Envelopes();
            var receiptData = new Receipts(accountId, user.EmployeeID);
            var output = new List<EnvelopeManagementDTONode>();
            var envelopeList = envelopeData.GetEnvelopesByAccount(accountId).Where(e => !e.ClaimId.HasValue && e.Status == EnvelopeStatus.PendingAdminReassignment);

            envelopeList.ForEach(e =>
            {
                var envelope = new EnvelopeManagementEnvelope(e);
                var receiptList = receiptData.GetByEnvelope(e.EnvelopeId, false);
                envelope.children = new List<EnvelopeManagementDTONode>(receiptList.Select(r => new EnvelopeManagementReceipt(r)));
                output.Add(envelope);
            });

            return output;
        }

        /// <summary>
        /// Uploads a receipt to the cloud, returning it's Id and temp url.
        /// </summary>
        /// <returns>An object in the format { id = 1234, url = "abc/1234.ext" }</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public object UploadReceiptToCloud()
        {
            var user = cMisc.GetCurrentUser();
            HttpPostedFile uploadedFile;

            if (HttpContext.Current.Request.Files.Count > 0)
            {
                uploadedFile = HttpContext.Current.Request.Files.Get(0);
            }
            else
            {
                throw new HttpException("Uploaded receipt not found. Are you uploading an image, document or text receipt?");
            }

            var filename = uploadedFile.FileName;
            var extension = Path.GetExtension(filename).ToLowerInvariant();
            var globalMimeTypes = new cGlobalMimeTypes(user.AccountID);
            var globalMimeType = globalMimeTypes.getMimeTypeByExtension(extension.Replace(".", ""));

            if (globalMimeType == null || globalMimeType.MimeHeader == null)
            {
                throw new HttpException("This file has an invalid file type. Images, documents and text are permitted.");
            }

            var accountMimeTypes = new cMimeTypes(user.AccountID, user.CurrentSubAccountId);
            var mimeType = accountMimeTypes.GetMimeTypeByGlobalID(globalMimeType.GlobalMimeID);

            if (mimeType == null)
            {
                throw new HttpException("This file has an unrecognisable file type.");
            }

            var reciepts = new Receipts(user.AccountID, user.EmployeeID);
            byte[] fileData;

            using (MemoryStream ms = new MemoryStream())
            {
                uploadedFile.InputStream.CopyTo(ms);
                fileData = ms.ToArray();
                if (!reciepts.CheckReceiptFileIsValid(fileData, extension))
                {
                    throw new ArgumentException("This file has been corrupted. This can be caused when a file is saved incorrectly and can restrict the file from opening.");
                }
            }

            if ((extension == ".pdf") && (user.Account.ValidationServiceEnabled == true))
            {
                var outputList = new List<object>();

                //convert to images and add paths to the ReceiptImageListView.Items.Add
                Image[] receiptImages;
                using (var pdfViewerControl = new PdfViewerControl())
                {
                    // Load the document in the viewer
                    pdfViewerControl.Load(uploadedFile.InputStream);
                    // Export the pages of the loaded document as bitmap images
                    receiptImages = pdfViewerControl.ExportAsImage(0, pdfViewerControl.PageCount - 1);
                }
                               
                foreach (Image image in receiptImages)
                {
                    var currentReceipt = new Receipt
                    {
                        CreationMethod = ReceiptCreationMethod.UploadedByClaimant,
                        Extension = "jpg",
                        ModifiedBy = user.EmployeeID,
                        ModifiedOn = DateTime.UtcNow
                    };

                    bool isCurrentReceiptImage;
                    using (var memoryStream = new MemoryStream())
                    {
                        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        currentReceipt = reciepts.AddReceipt(currentReceipt, memoryStream.ToArray());
                        isCurrentReceiptImage = reciepts.CheckIfReceiptIsImageAndOverwriteUrl(currentReceipt, memoryStream);
                    }

                    outputList.Add(new
                    {
                        id = currentReceipt.ReceiptId,
                        url = currentReceipt.TemporaryUrl,
                        isCurrentReceiptImage,
                        icon = currentReceipt.IconUrl
                    });
                }

                return new JavaScriptSerializer().Serialize(outputList);
            }

            // save and attach the receipt
            var data = new Receipts(user.AccountID, user.EmployeeID);
            var receipt = new Receipt
            {
                CreationMethod = ReceiptCreationMethod.UploadedByClaimant,
                Extension = extension,
                ModifiedBy = user.EmployeeID,
                ModifiedOn = DateTime.UtcNow
            };

            receipt = data.AddReceipt(receipt, fileData);
            
            var isImage = data.CheckIfReceiptIsImageAndOverwriteUrl(receipt, uploadedFile.InputStream);

            var output = new
            {
                id = receipt.ReceiptId,
                url = receipt.TemporaryUrl,
                isImage,
                icon = receipt.IconUrl
            };

            return new JavaScriptSerializer().Serialize(output);
        }
    }
}

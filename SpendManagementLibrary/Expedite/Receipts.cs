namespace SpendManagementLibrary.Expedite
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Data;
	using System.IO;
	using System.Globalization;
	using Interfaces.Expedite;
	using Helpers;
	using SELCloud;
	using SELCloud.Utilities;
	using Enumerators.Expedite;
	using SpendManagementLibrary.Flags;
	using SpendManagementLibrary.Helpers.AuditLogger;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Text;
    using System.Web;

	using BusinessLogic.FilePath;

    /// <summary>
	/// Manages all receipt processing in expenses.
	/// </summary>
	public class Receipts : IManageReceipts
	{
		#region Private Constants / Column Names / Param Keys / SQL

		private const string ColumnNameReceiptId = "ReceiptId";
		private const string ColumnNameReceiptFileExtension = "FileExtension";
		private const string ColumnNameReceiptCreationMethod = "CreationMethod";
		private const string ColumnNameReceiptCreatedOn = "CreatedOn";
		private const string ColumnNameReceiptCreatedBy = "CreatedBy";
		private const string ColumnNameReceiptModifiedOn = "ModifiedOn";
		private const string ColumnNameReceiptModifiedBy = "ModifiedBy";
		private const string ColumnNameReceiptExpediteUsername = "ExpediteUsername";
		private const string ColumnNameReceiptEnvelopeId = "EnvelopeId";
		private const string ColumnNameReceiptDeleted = "Deleted";
		private const string ColumnNameReceiptClaimId = "ClaimId";
		private const string ColumnNameReceiptUserId = "UserId";
		private const string ColumnNameSavedExpenseId = "SavedExpenseId";

		private const string ParamReceiptId = "@receiptId";
		private const string ParamReceiptFileExtension = "@fileExtension";
		private const string ParamReceiptCreationMethod = "@creationMethod";
		private const string ParamReceiptExpediteUsername = "@expediteUsername";
		private const string ParamReceiptEnvelopeId = "@envelopeId";
		private const string ParamReceiptClaimId = "@claimId";
		private const string ParamReceiptSavedExpenseId = "@savedExpenseId";
		private const string ParamReceiptEmployeeId = "@employeeId";
		private const string ParamReceiptUserId = "@userId";
		private const string ParamReceiptTrueDelete = "@trueDelete";

		private const string StoredProcAddReceipt = "AddReceipt";
		private const string StoredProcAddOrphanedReceipt = "AddOrphanedReceipt";
		private const string StoredProcAttachReceiptToClaimHeader = "AttachReceiptToClaimHeader";
		private const string StoredProcAttachReceiptToSavedExpense = "AttachReceiptToSavedExpense";
		private const string StoredProcAttachReceiptToUser = "AttachReceiptToUser";
		private const string StoredProcDetachReceiptFromSavedExpense = "DetachReceiptFromSavedExpense";
		private const string StoredProcDeleteReceipt = "DeleteReceipt";
		private const string StoredProcDeleteOrphanedReceipt = "DeleteOrphanedReceipt";
		private const string StoredProcGetCountOfExpensesWithNoReceipt = "CountValidatableExpensesInClaimWithNoReceipt";


		/*
			// Build the base SELECT SQL statement here - note the concat is done at compile time, not at runtime.
			// There is therefore no overhead, and this is (slightly) more readable than string builder is.
			// This is here so that there are not loads of strings littered throughout the class, and a column or param can be changed centrally.
			
				[ReceiptId]
				[FileExtension]
				[CreationMethod]
				[ClaimId]
				[UserId]
				[Deleted]
				[ExpediteUsername]
				[EnvelopeId]
				[CreatedOn]
				[CreatedBy]
				[ModifiedOn]
				[ModifiedBy]
		*/
		private const string BaseGetReceiptSql =
								"SELECT r." + ColumnNameReceiptId + ", r." + ColumnNameReceiptFileExtension + ", r." + ColumnNameReceiptCreationMethod +
								 ", r." + ColumnNameReceiptClaimId + ", r." + ColumnNameReceiptUserId + ", o." + ColumnNameSavedExpenseId + 
								 ", r." + ColumnNameReceiptExpediteUsername + ", r." + ColumnNameReceiptEnvelopeId + 
								 ", r." + ColumnNameReceiptCreatedOn + ", r." + ColumnNameReceiptCreatedBy + 
								 ", r." + ColumnNameReceiptModifiedOn + ", r." + ColumnNameReceiptModifiedBy +
								" FROM [dbo].[Receipts] AS r LEFT OUTER JOIN [dbo].[ReceiptOwnership] AS o ON r." 
								+ ColumnNameReceiptId + " = o." + ColumnNameReceiptId + " WHERE r." + ColumnNameReceiptDeleted + " = 0";

		private const string GetSingleReceiptSql = BaseGetReceiptSql + " AND r." + ColumnNameReceiptId + " = " + ParamReceiptId;
		private const string GetOrphanedReceiptSql = "SELECT * FROM [dbo].[OrphanedReceipts]";
		
		private static readonly InvalidDataException ReceiptErrorIdIsNotZero = new InvalidDataException("You must set the Id of a Receipt to 0 to add it.");
		private static readonly InvalidDataException ReceiptErrorNoData = new InvalidDataException("There is no data for the receipt.");
		
		#endregion Private SQL Constants / Column Names

		#region Private Receipt / Account Util Constants

		private int _accountId;
		private int _employeeId;
		private string _accountConnectionString;

		private const string IconDoc = "doc";
		private const string IconXls = "xls";
		private const string IconPpt = "ppt";
		private const string IconTif = "tiff";
		private const string IconPdf = "pdf";
		private const string IconTxt = "txt";
		private const string IconZip = "zip";
		private const string IconUnknown = "unknown";
		private const string IconString = "file_{0}.png";

		private static readonly List<string> ValidExtensions = new List<string>
		{
			"jpg", "jpeg", "jpe", "jp2",
			"png",
			"gif",
			"bmp"
		};

		private static readonly Dictionary<string, string> IconReplacements = new Dictionary<string, string>
		{
			{"doc", IconDoc}, {"docx", IconDoc}, {"odt", IconDoc}, {"fodt", IconDoc},
			{"xls", IconXls}, {"xlsx", IconXls}, {"ods", IconXls}, {"fods", IconXls},
			{"ppt", IconPpt}, {"pptx", IconPpt}, {"odp", IconPpt}, {"fodp", IconPpt}, 
			{"tif", IconTif}, {"tiff", IconTif}, {"odg", IconTif}, {"fodg", IconTif},
			{"pdf", IconPdf},
			{"txt", IconTxt}, {"csv", IconTxt}, {"asc", IconTxt}, {"log", IconTxt},
			{"zip", IconZip}, {"zipx", IconZip}, {"gzip", IconZip}, {"rar", IconZip}, {"jar", IconZip}, {"zz", IconZip}, {"7z", IconZip}, {"s7z", IconZip},
		};

		#endregion Private Receipt / Account Util Constants

		#region Constructor

		/// <summary>
		/// Creates a new Receipts instance.
		/// </summary>
		/// <param name="accountId">The accountId to identify the database.</param>
		/// <param name="employeeId">The employeeID to identify the current user.</param>
		public Receipts(int accountId, int employeeId)
		{
			AccountId = accountId;
			EmployeeId = employeeId;
		}

		#endregion Constructor

		#region Public Properties

		/// <summary>
		/// The Id of the Account.
		/// </summary>
		public int AccountId
		{
			get { return _accountId; }
			set
			{
				if (value < 1)
				{
					throw new ArgumentException();
				}
				_accountId = value;
				_accountConnectionString = cAccounts.getConnectionString(_accountId);
			}
		}

		/// <summary>
		/// The Id of the Employee.
		/// </summary>
		public int EmployeeId
		{
			get { return _employeeId; }
			set
			{
				if (value < 1)
				{
					throw new ArgumentException();
				}
				_employeeId = value;
			}
		}

		#endregion Public Properties

		#region Receipt Management
		
		/// <summary>
		/// Gets a single receipt by its ReceiptId.
		/// </summary>
		/// <param name="id">The database Id of the receipt.</param>
		/// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
		/// <returns>A list of Receipts.</returns>
		public Receipt GetById(int id, bool fetchFromCloud = true)
		{
			var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamReceiptId, id) };
			var list = ExtractReceiptsFromSql(GetSingleReceiptSql, args);
			if (fetchFromCloud) FetchFromCloudToTempStorageArea(list);
			return list.FirstOrDefault();
		}

		/// <summary>
		/// Gets the data from the cloud, in memory, and returns that data in a base-64 string.
		/// </summary>
		/// <param name="id">The database Id of the receipt.</param>
		/// <param name="isOrphan">Whether to look in the metabase for the receipt.</param>
		/// <returns>A base-64 string of the data.</returns>
		public string GetData(int id, bool isOrphan = false)
		{
			OrphanedReceipt firstItem;
			var list = new List<OrphanedReceipt>();
			
			if (isOrphan)
			{
				using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
				{
					// read the db
					using (IDataReader reader = connection.GetReader(GetOrphanedReceiptSql))
					{
						while (reader.Read())
						{
							// generate a new one
							list.Add(new OrphanedReceipt
							{
								ReceiptId = reader.GetRequiredValue<int>(ColumnNameReceiptId),
								Extension = reader.GetRequiredValue<string>(ColumnNameReceiptFileExtension),
								CreationMethod = reader.GetRequiredEnumValue<ReceiptCreationMethod>(ColumnNameReceiptCreationMethod)
							});
						}
					}
				}

				firstItem = list.FirstOrDefault();
			}
			else
			{
				var args = new List<KeyValuePair<string, object>> {new KeyValuePair<string, object>(ParamReceiptId, id)};
				firstItem = ExtractReceiptsFromSql(GetSingleReceiptSql, args).FirstOrDefault();
			}

			// do the fetch
			if (firstItem != null)
			{
				return FetchFromCloudIntoMemoryAndReturn(id, firstItem.Extension, isOrphan);
			}

			throw new InvalidDataException("A Receipt with this Id not found.");
		}

		/// <summary>
		/// Gets all receipts for an account that are not assigned to a user, claim, or claim line (savedexpense).
		/// Only an account administrator should have visibility of these.
		/// </summary>
		/// <returns>A list of Receipts.</returns>
		public IList<Receipt> GetUnassigned(bool fetchFromCloud = true)
		{
			const string sql = BaseGetReceiptSql + " AND " + ColumnNameSavedExpenseId + " IS NULL AND " + ColumnNameReceiptClaimId + " IS NULL AND " + ColumnNameReceiptUserId + " IS NULL ";
			var list = ExtractReceiptsFromSql(sql);
			if (fetchFromCloud) FetchFromCloudToTempStorageArea(list); FetchFromCloudToTempStorageArea(list);
			return list;
		}

	    /// <summary>
	    /// Gets all receipts for an envelope.
	    /// </summary>
	    /// <param name="envelopeId">The EnvelopeId of the envelope.</param>
	    /// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
	    /// <returns>A list of Receipts.</returns>
	    public IList<Receipt> GetByEnvelope(int envelopeId, bool fetchFromCloud = true)
	    {
	        const string sql = BaseGetReceiptSql + " AND " + ColumnNameReceiptEnvelopeId + " = " + ParamReceiptEnvelopeId;
	        var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamReceiptEnvelopeId, envelopeId) };
	        var list = ExtractReceiptsFromSql(sql, args);
	        if (fetchFromCloud) FetchFromCloudToTempStorageArea(list);
	        return list;
	    }

	    /// <summary>
	    /// Gets all receipts for a claim line (savedexpense).
	    /// </summary>
	    /// <param name="expenseItem">The <see cref="cExpenseItem"/>.</param>
	    /// <param name="currentUser">The <see cref="ICurrentUserBase"/>.</param>
	    /// <param name="subCategory">The <see cref="cSubcat"/>.</param>
	    /// <param name="claim">The <see cref="cClaim"/>.</param>
	    /// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
	    /// <returns>A list of Receipts.</returns>
	    public IList<Receipt> GetByClaimLine(cExpenseItem expenseItem, ICurrentUserBase currentUser, cSubcat subCategory, cClaim claim, bool fetchFromCloud = true)
		{
			const string sql = BaseGetReceiptSql + " AND " + ColumnNameSavedExpenseId + " = " + ParamReceiptSavedExpenseId;
			var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamReceiptSavedExpenseId, expenseItem.expenseid) };
			var list = ExtractReceiptsFromSql(sql, args);
          
		    this.AuditReceiptsViewed(list, expenseItem, currentUser, subCategory.subcat, claim, new AuditLogger());

            if (fetchFromCloud) FetchFromCloudToTempStorageArea(list);
			return list;
		}
       
        /// <summary>
        /// Creates and saves a thumbnail of the orginal receipt image. Returns the base64 string of the receipt's thumbnail image.
        /// </summary>
        /// <param name="receiptId">The Id of the receipt</param>
        /// <param name="extension">The extension of the receipt file</param>
        /// <returns>The base64 string of the receipt's thumbnail image</returns>
        public string GenerateThumbnailImageData(string receiptId, string extension)
        {
            if (extension.ToLower() == "pdf")
            {
                return Properties.Resources.PdfThumbnailIconData;
            }

            var globalFolderPaths = new GlobalFolderPaths();
            var mappedTempPath = globalFolderPaths.GetSingleFolderPath(_accountId, FilePathType.Receipt) + AccountId.ToString();

            var thumbnailFileName = receiptId + "-thumbnail.bmp";
            string thumbnailImagePath = string.Format("{0}\\{1}", mappedTempPath, thumbnailFileName);

            if (!File.Exists(thumbnailImagePath))
            {
                string filename = string.Format(@"\{0}.{1}", receiptId, extension);
                string receiptImagePath = mappedTempPath + filename;

                if (!File.Exists(receiptImagePath))
                {
                    return string.Empty;
                }
                
                Image thumbImage;
                // Load orginal receipt image for processing.
                using (Image image = Image.FromFile(receiptImagePath))
                {
                    thumbImage = this.ResizeImage(image, new Size(85, 85));
                }

                thumbImage.Save(thumbnailImagePath);             
            }

            Image receiptThumnailImage = this.GetImageFromPath(thumbnailImagePath);
            return this.ConvertImageToBase64(receiptThumnailImage, receiptThumnailImage.RawFormat);
        }

	    /// <summary>
	    /// Gets an image from the supplied path.
	    /// </summary>
	    /// <param name="path">
	    /// The path to the image.
	    /// </param>
	    /// <returns>
	    /// The <see cref="Image"/>.
	    /// </returns>
	    public Image GetImageFromPath(string path)
	    {
	        Image image = Image.FromFile(path);
	        return image;
	    }

	    /// <summary>
	    /// Converts the supplied image to a base64 string 
	    /// </summary>
	    /// <param name="image">
	    /// The image to convert.
	    /// </param>
	    /// <param name="format">
	    /// The image's format.
	    /// </param>
	    /// <returns>
	    /// The base64 <see cref="string"/>.
	    /// </returns>
	    public string ConvertImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
	    {
	        using (MemoryStream ms = new MemoryStream())
	        {
	            image.Save(ms, format);
	            byte[] imageBytes = ms.ToArray();
	            string base64String = Convert.ToBase64String(imageBytes);
	            return base64String;
	        }
	    }

        /// <summary>
        /// Resizes an image from the centre of the supplied image.
        /// </summary>
        /// <param name="imgToResize">The image to resize</param>
        /// <param name="newDimension">The new dimensions</param>
        /// <returns>The resized image</returns>
        private Image ResizeImage(Image imgToResize, Size newDimension)
        {
            int originalWidth = imgToResize.Width;
            int originalHeight = imgToResize.Height;
            float hRatio = (float)originalHeight / newDimension.Height;
            float wRatio = (float)originalWidth / newDimension.Width;
            float ratio = Math.Min(hRatio, wRatio);
            int hScale = Convert.ToInt32(newDimension.Height * ratio);
            int wScale = Convert.ToInt32(newDimension.Width * ratio);

            //start cropping from the center
            var startX = (originalWidth - wScale) / 2;
            var startY = (originalHeight - hScale) / 2;

            //crop the image from the specified location and size
            var sourceRectangle = new Rectangle(startX, startY, wScale, hScale);
  
            var bitmap = new Bitmap(newDimension.Width, newDimension.Height);
        
            var destinationRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            //generate the new image
            using (Graphics graphic = Graphics.FromImage(bitmap))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.DrawImage(imgToResize, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        /// <summary>
		/// Gets all receipts for a claim.
		/// </summary>
		/// <param name="claimId">The ClaimId of the row in the claims table.</param>
		/// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
		/// <returns>A list of Receipts.</returns>
		public IList<Receipt> GetByClaim(int claimId, bool fetchFromCloud = true)
		{
			const string sql = BaseGetReceiptSql + " AND " + ColumnNameReceiptClaimId + " = " + ParamReceiptClaimId;
			var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamReceiptClaimId, claimId) };
			IList<Receipt> list = ExtractReceiptsFromSql(sql, args);
			if (fetchFromCloud) FetchFromCloudToTempStorageArea(list);
			return list;
		}

		/// <summary>
		/// Gets all receipts for a claim line (savedexpense).
		/// </summary>
		/// <param name="employeeId">The EmployeeId of the row in the Employees table.</param>
		/// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
		/// <returns>A list of Receipts.</returns>
		public IList<Receipt> GetByClaimant(int employeeId, bool fetchFromCloud = true)
		{
			const string sql = BaseGetReceiptSql + " AND " + ColumnNameReceiptUserId + " = " + ParamReceiptUserId;
			var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamReceiptUserId, employeeId) };
			var list = ExtractReceiptsFromSql(sql, args);
			if (fetchFromCloud) FetchFromCloudToTempStorageArea(list);
			return list;
		}

		/// <summary>
		/// Gets all orphaned receipts from the metabase. 
		/// These are receipts where we cannot identify the account, which means we 
		/// don't know the claim, claimant, claim line, or even account.
		/// </summary>
		/// <param name="fetchFromCloud">Whether to attempt to fetch from the cloud.</param>
		/// <returns>A list of OrphanedReceipts.</returns>
		public IList<OrphanedReceipt> GetOrphaned(bool fetchFromCloud = true)
		{
			var receipts = new List<OrphanedReceipt>();

			using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
			{
				// read the db
				using (IDataReader reader = connection.GetReader(GetOrphanedReceiptSql))
				{
					while (reader.Read())
					{
						// generate a new one
						receipts.Add(new OrphanedReceipt
						{
							ReceiptId = reader.GetRequiredValue<int>(ColumnNameReceiptId),
							Extension = reader.GetRequiredValue<string>(ColumnNameReceiptFileExtension),
							CreationMethod = reader.GetRequiredEnumValue<ReceiptCreationMethod>(ColumnNameReceiptCreationMethod)
						});
					}
				}
			}
			FetchFromCloudToTempStorageArea(receipts, true);
			return receipts;
		}

		/// <summary>
		/// Checks whether all the claim lines (savedexpense) for a given claim id have receipts.
		/// </summary>
		/// <param name="claimId">The Id of the claim.</param>
		/// <returns>A bool, indicating the result.</returns>
		public bool CheckIfAllValidatableClaimLinesHaveReceiptsAttached(int claimId)
		{
			// pass the claimId as argument.
			var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamReceiptClaimId, claimId) };
			var count = RunSingleStoredProcedure(StoredProcGetCountOfExpensesWithNoReceipt, args);
			return count == 0;
		}

		/// <summary>
		/// Adds a receipt to the account you initialised this class with.
		/// Linkages will be ignored. Please use the link methods.
		/// </summary>
		/// <param name="receipt">The receipt to add.</param>
		/// <param name="data">The actual binary data for the receipt.</param>
		/// <returns>The added receipt.</returns>
		public Receipt AddReceipt(Receipt receipt, byte[] data)
		{
			if (receipt.ReceiptId != 0)
			{
				throw ReceiptErrorIdIsNotZero;
			}

			if (data == null || data.Length == 0)
			{
				throw ReceiptErrorNoData;
			}

			// attempt conversion to jpg (changes the file extension of the receipt)
			var conversion = AttemptConversionToJpg(receipt, data);

			// first add to the database, grabbing the returned id.
			var args = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>(ParamReceiptFileExtension, receipt.Extension.Replace(".", "").ToLower()),
				new KeyValuePair<string, object>(ParamReceiptCreationMethod, receipt.CreationMethod),
				new KeyValuePair<string, object>(ParamReceiptUserId, EmployeeId)
			};

			if (receipt.ExpediteInfo != null)
			{
				args.Add(new KeyValuePair<string, object>(ParamReceiptEnvelopeId, receipt.ExpediteInfo.EnvelopeId));
				args.Add(new KeyValuePair<string, object>(ParamReceiptExpediteUsername, receipt.ExpediteInfo.ExpediteUserName));
			}

			receipt.ReceiptId = RunSingleStoredProcedure(StoredProcAddReceipt, args);
             receipt.OwnershipInfo = new ReceiptOwnershipInfo();

            return (Receipt)AttemptSaveToCloud(receipt, conversion);
		}
		
		/// <summary>
		/// Adds an <see cref="OrphanedReceipt"/> to the metabase.
		/// This should only be called by Expedite staff when the receipt has no identifying info.
		/// </summary>
		/// <param name="receipt">The orphaned receipt.</param>
		/// <param name="data">The actual binary data for the file.</param>
		/// <returns>The added orphan.</returns>
		public OrphanedReceipt AddOrphan(OrphanedReceipt receipt, byte[] data)
		{
			if (receipt.ReceiptId != 0)
			{
				throw ReceiptErrorIdIsNotZero;
			}

			if (data == null || data.Length == 0)
			{
				throw ReceiptErrorNoData;
			}

			// attempt conversion to jpg (changes the file extension of the receipt)
			var conversion = AttemptConversionToJpg(receipt, data);

			var args = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>(ParamReceiptFileExtension, receipt.Extension),
				new KeyValuePair<string, object>(ParamReceiptCreationMethod, receipt.CreationMethod)
			};
			receipt.ReceiptId = RunSingleStoredProcedure(StoredProcAddOrphanedReceipt, args, true);

			// cloud save
			return AttemptSaveToCloud(receipt, conversion, true);
		}

		/// <summary>
		/// Links a receipt to a claimant (employee). 
		/// Assumes that both the Receipt and Claimant exists.
		/// Calling this will remove any ClaimLine-level links, and any claim link.
		/// </summary>
		/// <param name="receiptId">The ReceiptId of the receipt.</param>
		/// <param name="employeeId">The EmployeeId of the row in the Employees table.</param>
		/// <returns></returns>
		public Receipt LinkToClaimant(int receiptId, int employeeId)
		{
			// create the params for the statement
			var args = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>(ParamReceiptId, receiptId),
				new KeyValuePair<string, object>(ParamReceiptEmployeeId, employeeId),
				new KeyValuePair<string, object>(ParamReceiptUserId, EmployeeId)
			};

			// do the save
			RunSingleStoredProcedure(StoredProcAttachReceiptToUser, args);

			// remove the employeeId from the args
			args.RemoveAt(1);

			// do a get on the receipt.
			return ExtractReceiptsFromSql(GetSingleReceiptSql, args).First();
		}

		/// <summary>
		/// Links a receipt to a claim header. 
		/// A receipt can only be placed against one claim.
		/// Calling this will remove any ClaimLine-level links, and any claimant link.
		/// </summary>
		/// <param name="receiptId">The ReceiptId of the receipt.</param>
		/// <param name="claimId">The ClaimId of the row in the claims table.</param>
		/// <returns>The linked receipt.</returns>
		public Receipt LinkToClaim(int receiptId, int claimId)
		{
			// create the params for the statement
			var args = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>(ParamReceiptId, receiptId),
				new KeyValuePair<string, object>(ParamReceiptClaimId, claimId),
				new KeyValuePair<string, object>(ParamReceiptUserId, EmployeeId)
			};

			// do the save
			RunSingleStoredProcedure(StoredProcAttachReceiptToClaimHeader, args);

			// remove the claimId from the args
			args.RemoveAt(1);

			// do a get on the receipt.
			return ExtractReceiptsFromSql(GetSingleReceiptSql, args).First();
		}

		/// <summary>
		/// Links a receipt to a claim line. Call this multiple times if needed.
		/// Receipts can now be stored against multiple lines. A line can also have multiple receipts.
		/// Calling this will remove any Claim link, and any claimant link, but not other claimline links.
		/// To remove a ClaimLine-level link, use <see cref="IManageReceipts.UnlinkFromClaimLine"/>.
		/// </summary>
		/// <param name="receiptId">The ReceiptId of the receipt.</param>
		/// <param name="savedExpenseId">The ExpenseId of the row in the savedexpenses table.</param>
		/// <returns>The linked receipt.</returns>
		public Receipt LinkToClaimLine(int receiptId, int savedExpenseId)
		{
			// create the params for the statement
			var args = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>(ParamReceiptId, receiptId),
				new KeyValuePair<string, object>(ParamReceiptSavedExpenseId, savedExpenseId),
				new KeyValuePair<string, object>(ParamReceiptUserId, EmployeeId)
			};

			// do the save
			RunSingleStoredProcedure(StoredProcAttachReceiptToSavedExpense, args);

			// remove the savedExpenseId from the args
			args.RemoveAt(1);

			//remove any receipt not attached flags
			Flags flagMan = new Flags(this.AccountId);
			flagMan.DeleteReceiptNotAttachedFlag(savedExpenseId);

			// do a get on the receipt.
			return ExtractReceiptsFromSql(GetSingleReceiptSql, args).First();
		}

		/// <summary>
		/// Removes the links between a receipt and a claim line. Call this multiple times if needed.
		/// Receipts can now be stored against multiple lines. A line can also have multiple receipts.
		/// </summary>
		/// <param name="receiptId">The ReceiptId of the receipt.</param>
		/// <param name="savedExpenseId">The ExpenseId of the row in the savedexpenses table.</param>
		/// <returns>The modified receipt.</returns>
		public Receipt UnlinkFromClaimLine(int receiptId, int savedExpenseId)
		{
			// create the params for the statement
			var args = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>(ParamReceiptId, receiptId),
				new KeyValuePair<string, object>(ParamReceiptSavedExpenseId, savedExpenseId),
				new KeyValuePair<string, object>(ParamReceiptUserId, EmployeeId)
			};

			// do the save
			RunSingleStoredProcedure(StoredProcDetachReceiptFromSavedExpense, args);

			// remove the savedExpenseId from the args
			args.RemoveAt(1);

			// do a get on the receipt.
			return ExtractReceiptsFromSql(GetSingleReceiptSql, args).First();
		}

		/// <summary>
		/// Updates a batch of receipt linkages in one go.
		/// </summary>
		/// <param name="toRemove">A dictionary of ReceiptIds, with their accompanying linkages TO BE REMOVED.</param>
		/// <param name="toAssign">A dictionary of ReceiptIds, with their accompanying linkages TO BE ADDED.</param>
		/// <returns>The modified receipt.</returns>
		public void BatchUpdateReceiptLinkages(Dictionary<int, ReceiptOwnershipInfo> toRemove, Dictionary<int, ReceiptOwnershipInfo> toAssign)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Takes an orphaned receipt from the metabase, and adds it to the account you initialised this class with.
		/// This is achieved by getting the file from the cloud, generating a new receipt in the specified account,
		/// taking the newly generated Id and renaming and re-uploading the renamed file so there is a copy in the account
		/// database and folder structure in the cloud. Finally, <see cref="IManageReceipts.DeleteOrphan"/> is called on the old Id.
		/// This will remove the receipt from the metabase and the file itself from the cloud.
		/// </summary>
		/// <param name="id">The Id of the OrphanedReceipt.</param>
		/// <returns></returns>
		public Receipt MoveOrphanToAccount(int id)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Deletes a <see cref="Receipt"/>. Note that we don't actually delete normal Receipts. 
		/// A deleted flag is set against the receipt and it will not show up in results.
		/// </summary>
		/// <param name="id">The Id of the Receipt to delete.</param>
		/// <param name="actualDelete">Whether to actually delete the receipt. 
		/// Do not use unless you are certain it is legal to delete the receipt.</param>
		public void DeleteReceipt(int id, bool actualDelete = false)
		{
			// create the params for the statement
			var args = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>(ParamReceiptId, id),
				new KeyValuePair<string, object>(ParamReceiptTrueDelete, actualDelete),
				new KeyValuePair<string, object>(ParamReceiptUserId, EmployeeId)
			};

			// do the save
			RunSingleStoredProcedure(StoredProcDeleteReceipt, args);
		}

		/// <summary>
		/// Deletes an <see cref="OrphanedReceipt"/>. Note that this IS permanent and removes both
		/// the metabase entry and the file in the cloud. Call this only if you are SURE the receipt
		/// is allowed to be deleted.
		/// </summary>
		/// <param name="id">The Id of the OrphanedReceipt to delete.</param>
		public void DeleteOrphan(int id)
		{
			// create the params for the statement
			var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamReceiptId, id) };

			// do the save
			RunSingleStoredProcedure(StoredProcDeleteOrphanedReceipt, args, true);
		}

		/// <summary>
		/// Determines whether the supplied receipt is actually an image that can be web displayed.
		/// Attempts to convert the data of a file to an image, if data is supplied.
		/// If this fails then the extension is looked up, and the supplied receipt's iconUrl is populated.
		/// </summary>
		/// <param name="receipt">The receipt.</param>
		/// <param name="data">The receipt's data.</param>
		/// <returns>Whether the receipt is an image.</returns>
		public bool CheckIfReceiptIsImageAndOverwriteUrl(Receipt receipt, Stream data = null)
		{
			var receiptIsImage = true;

			try
			{
				if (data == null)
				{
					throw new Exception("No data");
				}

				System.Drawing.Image.FromStream(data);
			}
			catch
			{
				// in case for some reason data wasn't provided (eg. on a get, not upload,) then 
				// check the extension against a list of acceptable extensions.
				if (!ValidExtensions.Contains(receipt.Extension.ToLower()))
				{
					// assign the icon from the dictionary / fallback
					string icon;

					if (!IconReplacements.TryGetValue(receipt.Extension.ToLower(), out icon))
					{
						icon = IconUnknown;
					}

					icon = string.Format(IconString, icon);
					icon = string.Format("{0}/icons/Custom/128/{1}", GlobalVariables.StaticContentLibrary, icon);
					receipt.IconUrl = icon;
					receiptIsImage = false;
				}
			}

			return receiptIsImage;
		}

		#endregion Receipt Management

		#region Private Helpers

		/// <summary>
		/// Reads the database and builds receipts for each row in the query result.
		/// </summary>
		/// <param name="sql">The SQL to execute.</param>
		/// <param name="addWithValueArgs">Any arguments to pass to the SQL.</param>
		/// <returns>A list of receipts.</returns>
		private IList<Receipt> ExtractReceiptsFromSql(string sql, List<KeyValuePair<string, object>> addWithValueArgs = null)
		{
			var receipts = new List<Receipt>();

			using (var connection = new DatabaseConnection(_accountConnectionString))
			{
				// add arguments to the connection
				if (addWithValueArgs != null)
				{
					addWithValueArgs.ForEach(x => connection.AddWithValue(x.Key, x.Value));
				}

				// read the db
				using (IDataReader reader = connection.GetReader(sql))
				{
					while (reader.Read())
					{
						var tempId = reader.GetRequiredValue<int>(ColumnNameReceiptId);
						var existing = receipts.FirstOrDefault(x => x.ReceiptId == tempId);
						var claimLineId = reader.GetNullable<int>(ColumnNameSavedExpenseId);
						int? envelopeId = reader.GetNullable<int>(ColumnNameReceiptEnvelopeId);
						var expediteUserName = reader.GetValueOrDefault<string>(ColumnNameReceiptExpediteUsername, null);

						// the row might already correspond to another receipt
						if (existing == null)
						{
							// generate a new one
							var newReceipt = new Receipt
							{
								ReceiptId = tempId,
								Extension = reader.GetRequiredValue<string>(ColumnNameReceiptFileExtension),
								CreationMethod = reader.GetRequiredEnumValue<ReceiptCreationMethod>(ColumnNameReceiptCreationMethod),
								CreatedOn = reader.GetRequiredValue<DateTime>(ColumnNameReceiptCreatedOn),
								CreatedBy = reader.GetRequiredValue<int>(ColumnNameReceiptCreatedBy),
								ModifiedOn = reader.GetNullable<DateTime>(ColumnNameReceiptModifiedOn),
								ModifiedBy = reader.GetNullable<int>(ColumnNameReceiptModifiedBy),
								OwnershipInfo = new ReceiptOwnershipInfo
								{
									ClaimLines = new List<int>(),
									ClaimId = reader.GetNullable<int>(ColumnNameReceiptClaimId),
									EmployeeId = reader.GetNullable<int>(ColumnNameReceiptUserId)
								}
							};

							if (envelopeId.HasValue || !string.IsNullOrEmpty(expediteUserName))
							{
								
									newReceipt.ExpediteInfo = new ExpediteInfo
									{
										EnvelopeId = envelopeId ?? 0,
										ExpediteUserName = expediteUserName
									};
							}

							// set the array if there is a value
							if (claimLineId.HasValue)
							{
								newReceipt.OwnershipInfo.ClaimLines.Add(claimLineId.Value);
							}

							// add to the list
							receipts.Add(newReceipt);
						}
						else
						{
							// Otherwise we already have a receipt by this Id,
							// so we have to just update the claim lines
							if (claimLineId.HasValue)
							{
								existing.OwnershipInfo.ClaimLines.Add(claimLineId.Value);
							}
						}
					}
				}

				connection.ClearParameters();
			}
			return receipts;
		}

		/// <summary>
		/// Executes a stored procedure on a single item, returning the result.
		/// By default the connection string is used for the account db.
		/// </summary>
		/// <param name="sql">The name of the stored procedure.</param>
		/// <param name="addWithValueArgs">The arguments to pass to the stored procedure.</param>
		/// <param name="connectToMetabase">Connects to the metabase (rather than account db) if you pass true.</param>
		/// <returns>An integer result.</returns>
		private int RunSingleStoredProcedure(string sql, List<KeyValuePair<string, object>> addWithValueArgs = null, bool connectToMetabase = false)
		{
			const string returnValueKey = "returnvalue";
			int result;

			using (var connection = new DatabaseConnection(connectToMetabase ? GlobalVariables.MetabaseConnectionString : _accountConnectionString))
			{
				if (addWithValueArgs != null)
				{
					addWithValueArgs.ForEach(x => connection.AddWithValue(x.Key, x.Value));
				}

				connection.AddReturn(returnValueKey, SqlDbType.Int);
				connection.ExecuteProc(sql);
				result = connection.GetReturnValue<int>(returnValueKey);

				if (result < 0)
				{
					throw new InvalidDataException("Item not saved.");
				}

				connection.ClearParameters();
			}

			return result;
		}

		/// <summary>
		/// Fetches a list of receipts from their place on the cloud or filesystem, to a 
		/// temporary folder for serving to a client, either through the API or the Web UI.
		/// The special property TemporaryUrl will be populated.
		/// </summary>
		/// <param name="receipts">The list of receipts</param>
		/// <param name="isOrphanedReceipt">Whether the receipts are orphans. They must all be.</param>
		private void FetchFromCloudToTempStorageArea(IEnumerable<OrphanedReceipt> receipts, bool isOrphanedReceipt = false)
		{
			// create the temp folder in the receipts folder if it doesn't exist.
			var accountString = isOrphanedReceipt ? "SEL" : _accountId.ToString(CultureInfo.InvariantCulture);
			var globalFolderPaths = new GlobalFolderPaths();
			var mappedTempPath = globalFolderPaths.GetSingleFolderPath(_accountId, FilePathType.Receipt) + accountString;
			var originalMappedTempPath = mappedTempPath.Clone();
			
			if (!Directory.Exists(mappedTempPath))
			{
				Directory.CreateDirectory(mappedTempPath);
			}

			// Fetch the data from RackSpace, saving in the receipts folder (which is now cleared every night)
			foreach (var receipt in receipts)
			{
				var receiptFilename = receipt.ReceiptId + "." + receipt.Extension;
                var virtualTempPath = $"getdocument.axd?docType={2}&receiptId={receipt.ReceiptId}"; // docType 2 is for receipts
				mappedTempPath = string.Format(originalMappedTempPath + "\\{0}", receiptFilename);

				// only fetch if not there.
				if (!File.Exists(mappedTempPath))
				{
					try
					{
						SELCloud.Instance.GetObject(accountString, @"Receipts\" + receiptFilename, mappedTempPath);
					}
					catch (Exception)
					{
						virtualTempPath = "static/icons/custom/128/file_notfound.png";
					}
				}

				receipt.TemporaryUrl = virtualTempPath;
			}
		}

		/// <summary>
		/// Fetches a receipt from the cloud, into memory, then returns the resultant data in the form of a base-64 string.
		/// </summary>
		/// <param name="id">The Id of the receipt.</param>
		/// <param name="extension">The file extension for the receipt.</param>
		/// <param name="isOrphanedReceipt">Whether the receipt is an orphan.</param>
		/// <returns>The data in base-64 format.</returns>
		private string FetchFromCloudIntoMemoryAndReturn(int id, string extension, bool isOrphanedReceipt = false)
		{
			var stream = new MemoryStream();
			var receiptFilename = id + "." + extension;
			var accountString = isOrphanedReceipt ? "SEL" : _accountId.ToString(CultureInfo.InvariantCulture);
			SELCloud.Instance.GetObject(accountString, @"Receipts\" + receiptFilename, stream);
			var output = Convert.ToBase64String(stream.GetBuffer(), Base64FormattingOptions.None);

			return output;
		}

		/// <summary>
		/// Attempts to convert the binary data from the file to a jpg if it is an image type.
		/// Otherwise, there will be no difference between the result and the original.
		/// </summary>
		/// <param name="receipt">The receipt to save.</param>
		/// <param name="data">The actual binary data for the file.</param>
		/// <returns>The conversion information..</returns>
		private ImageMemoryConversion AttemptConversionToJpg(OrphanedReceipt receipt, byte[] data)
		{
			// strip off any dot in the extension.
			receipt.Extension = receipt.Extension.Replace(".", "");

			// create a memory stream of the data to push to the data store
			var memoryStream = new MemoryStream(data, 0, data.Length);

			// convert receipt to jpeg (if it is of an image type)
			var conversion = ImageConvertor.ImageToJpg(memoryStream, new MemoryStream(), 80);
			if (!conversion.ConvertedFile.Equals(conversion.SourceFile))
			{
				receipt.Extension = "jpg";
			}

			return conversion;
		}

		/// <summary>
		/// Attempts to save a receipt to the cloud, or rather the SELCloud, which can also be the filesystem.
		/// </summary>
		/// <param name="receipt">The receipt to save.</param>
		/// <param name="conversion">The ImageMemoryConversion of the file.</param>
		/// <param name="receiptIsOrphan">Whether to upload the receipt to a special folder for orphans.</param>
		/// <returns>The same receipt, or null if there was an error.</returns>
		private OrphanedReceipt AttemptSaveToCloud(OrphanedReceipt receipt, ImageMemoryConversion conversion, bool receiptIsOrphan = false)
		{
			// build the file name
			var fileName = @"Receipts\" + receipt.ReceiptId + "." + receipt.Extension;

			try
			{
				// generate the container (if not exists)
				var folderPath = receiptIsOrphan ? "SEL" : _accountId.ToString(CultureInfo.InvariantCulture);
				SELCloud.Instance.CreateContainer(folderPath);

				// attempt to save the file in the data store
				SELCloud.Instance.CreateObject(folderPath, fileName, conversion.ConvertedFile);
			}
			catch (Exception)
			{
				// roll back receipt attachment
				DeleteReceipt(receipt.ReceiptId, true);

				return null;
			}

			return receiptIsOrphan ? GetOrphaned().FirstOrDefault(o => o.ReceiptId == receipt.ReceiptId) : GetById(receipt.ReceiptId);
		}

        /// <summary>
        /// Adds an entry to the audit log for the receipts that have been viewed.
        /// </summary>
        /// <param name="receipts">The list of <see cref="Receipt"/> to audit.</param>
        /// <param name="expense">The <see cref="cExpenseItem"/>.</param>
        /// <param name="currentUser">The <see cref="ICurrentUserBase"/>.</param>
        /// <param name="subCategoryName">The sub category name the expense belongs to.</param>
        /// <param name="claim">>The <see cref="cClaim"/>.</param></param>
        /// <param name="auditLogger">The <see cref="IAuditLogger"/>.</param>
        private void AuditReceiptsViewed(IList<Receipt> receipts, cExpenseItem expense, ICurrentUserBase currentUser, string subCategoryName, cClaim claim, IAuditLogger auditLogger)
	    {
	        if (currentUser.EmployeeID != claim.employeeid || (currentUser.isDelegate && currentUser.Delegate.EmployeeID != claim.employeeid))
	        {	        
	            foreach (var receipt in receipts)
	            {
	                string record =
	                    $"Receipt ({receipt.ReceiptId}) attached to expense item ({expense.refnum} {subCategoryName} {expense.date.ToShortDateString()}), claim ({claim.name})";

	                auditLogger.ViewRecordAuditLog(currentUser, SpendManagementElement.Receipts, record);             
	            }
	        }
	    }

        /// <summary>
        /// Check if the Receipt file is valid or not . 
        /// If the file has valid extension but the file might be created by modifying any file by changing the extension.
        /// </summary>
        /// <param name="documentStream"></param>
        /// <param name="extension"></param>
        /// <returns>flag if the file is valid</returns>
        public bool CheckReceiptFileIsValid(byte[] documentStream, string extension)
        {          
            try
            {
                if (extension.ToLower() == ".pdf")
                {
                  return IsPDFHeader(documentStream);
                }

                //Executed for the file with valid extension
                if (ValidExtensions.Contains(extension.Replace(".", "").ToLower()))                  
                {   
                using (MemoryStream documentMemoryStream = new MemoryStream(documentStream))
                    Image.FromStream(documentMemoryStream);
                }               
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }       

        /// <summary>
        /// Check the header of the file stream to check if the file is valid pdf file
        /// </summary>
        /// <param name="pdfStream"></param>
        /// <returns></returns>
        public bool IsPDFHeader(byte[] pdfStream)
        {
            try
            {
                var encoding = new ASCIIEncoding();
                var header = encoding.GetString(pdfStream);
                if (pdfStream[0] == 0x25 && pdfStream[1] == 0x50
                    && pdfStream[2] == 0x44 && pdfStream[3] == 0x46)
                {
                    return header.StartsWith("%PDF-");
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }          
        }

        #endregion Private Helpers
    }
}

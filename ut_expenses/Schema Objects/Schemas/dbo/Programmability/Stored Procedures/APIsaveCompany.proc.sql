CREATE PROCEDURE [dbo].[APIsaveCompany]
			@companyid int out
		   ,@company nvarchar(250)
           ,@archived bit
           ,@comment nvarchar(4000)
           ,@companycode nvarchar(60)
           ,@showfrom bit
           ,@showto bit
           ,@CreatedOn datetime
           ,@CreatedBy int
           ,@ModifiedOn datetime
           ,@ModifiedBy int
           ,@address1 nvarchar(250)
           ,@address2 nvarchar(250)
           ,@city nvarchar(250)
           ,@county nvarchar(250)
           ,@postcode nvarchar(250)
           ,@country int
           ,@parentcompanyid int
           ,@iscompany bit
           ,@CacheExpiry datetime
           ,@addressCreationMethod tinyint
           ,@isPrivateAddress bit
           ,@addressLookupDate datetime
           ,@subAccountID int
           ,@ESRLocationId bigint
           ,@address3 nvarchar(250)
           ,@ESRAddressId bigint
AS
BEGIN
	IF @companyid = 0 AND EXISTS (SELECT companyid FROM COMPANIES WHERE company = @company)
	BEGIN
		SELECT @companyid = companyid FROM COMPANIES WHERE company = @company;
	END

	IF @companyid = 0 AND EXISTS (SELECT companyid FROM COMPANIES WHERE address1 = @address1 AND postcode = @postcode)
	BEGIN
		SELECT @companyid = companyid, @company = company  FROM COMPANIES WHERE address1 = @address1 AND postcode = @postcode;
	END

	IF @companyid = 0
	BEGIN
		IF EXISTS (SELECT companyid FROM companies WHERE [company] = @company)
		BEGIN
			RETURN -1;
		END
		
		INSERT INTO [dbo].[companies]
           ([company]
           ,[archived]
           ,[comment]
           ,[companycode]
           ,[showfrom]
           ,[showto]
           ,[CreatedOn]
           ,[CreatedBy]
           ,[ModifiedOn]
           ,[ModifiedBy]
           ,[address1]
           ,[address2]
           ,[city]
           ,[county]
           ,[postcode]
           ,[country]
           ,[parentcompanyid]
           ,[iscompany]
           ,[CacheExpiry]
           ,[addressCreationMethod]
           ,[isPrivateAddress]
           ,[addressLookupDate]
           ,[subAccountID]
           ,[address3]
			)
     VALUES
           (@company 
           ,@archived
           ,@comment 
           ,@companycode 
           ,@showfrom 
           ,@showto 
           ,@CreatedOn
           ,@CreatedBy 
           ,@ModifiedOn 
           ,@ModifiedBy 
           ,@address1 
           ,@address2 
           ,@city 
           ,@county 
           ,@postcode 
           ,@country 
           ,@parentcompanyid 
           ,@iscompany 
           ,@CacheExpiry 
           ,@addressCreationMethod 
           ,@isPrivateAddress 
           ,@addressLookupDate 
           ,@subAccountID 
           ,@address3 
		   )
		   	set @companyid = scope_identity();
		END
	ELSE
		BEGIN
		UPDATE [dbo].[companies]
		   SET [company] = @company 
			  ,[archived] = @archived 
			  ,[comment] = @comment 
			  ,[companycode] = @companycode 
			  ,[showfrom] = @showfrom 
			  ,[showto] = @showto 
			  ,[CreatedOn] = @CreatedOn 
			  ,[CreatedBy] = @CreatedBy 
			  ,[ModifiedOn] = @ModifiedOn 
			  ,[ModifiedBy] = @ModifiedBy 
			  ,[address1] = @address1 
			  ,[address2] = @address2 
			  ,[city] = @city 
			  ,[county] = @county 
			  ,[postcode] = @postcode 
			  ,[country] = @country 
			  ,[parentcompanyid] = @parentcompanyid 
			  ,[iscompany] = @iscompany 
			  ,[CacheExpiry] = @CacheExpiry 
			  ,[addressCreationMethod] = @addressCreationMethod 
			  ,[isPrivateAddress] = @isPrivateAddress 
			  ,[addressLookupDate] = @addressLookupDate 
			  ,[subAccountID] = @subAccountID 
			  ,[address3] = @address3 
		 WHERE [companyid] = @companyid
		 
		END

		IF NOT EXISTS (SELECT * FROM CompanyEsrAllocation WHERE companyid = @companyid AND ISNULL(ESRLocationID, '') = ISNULL(@ESRLocationId, '')  AND ISNULL(ESRAddressID, '') = ISNULL(@esraddressid, '') )
		BEGIN
			INSERT INTO CompanyEsrAllocation (companyid, ESRLocationID, ESRAddressID) VALUES (@companyid, @ESRLocationId, @ESRAddressId)
		END
END
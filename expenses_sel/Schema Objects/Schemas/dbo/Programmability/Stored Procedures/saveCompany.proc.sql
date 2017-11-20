--ALTER TABLE dbo.companies ADD isPrivateAddress bit NOT NULL CONSTRAINT DF_companies_isPrivateAddress DEFAULT 0

CREATE PROCEDURE [dbo].[saveCompany]
@companyid int,
@company nvarchar(250),
@comment nvarchar(4000),
@companycode NVARCHAR(50),
@showfrom BIT,
@showto BIT,
@address1 NVARCHAR(250),
@address2 NVARCHAR(250), @city NVARCHAR(250),
@county NVARCHAR(250),
@postcode NVARCHAR(250),
@country INT,
@parentcompanyid INT,
@iscompany BIT,
@isPrivateAddress BIT,
@addressCreationMethod TINYINT,
@userid INT,
@date datetime,
@delegateID INT

AS

DECLARE @count INT;

	
if (@companyid = 0)
BEGIN
	SET @count = (SELECT COUNT(*) FROM companies WHERE company = @company);
	IF @count > 0
		RETURN -1;
		
	insert into companies (company, comment, companycode, showfrom, showto, createdon, createdby, address1, address2, city, county, postcode, country, parentcompanyid, iscompany, isPrivateAddress, addressCreationMethod)
		VALUES (@company, @comment, @companycode, @showfrom, @showto, @date, @userid, @address1, @address2, @city, @county, @postcode, @country, @parentcompanyid, @iscompany, @isPrivateAddress, @addressCreationMethod);
	set @companyid = scope_identity();
	
	if @userid > 0
	Begin
		exec addInsertEntryToAuditLog @userid, @delegateID, 38, @companyid, @company, null;
	end
end
else
BEGIN
	SET @count = (SELECT COUNT(*) FROM companies WHERE company = @company AND companyid <> @companyid);
	IF @count > 0
		RETURN -1;

	DECLARE @oldcompany nvarchar(250);
	DECLARE @oldcomment nvarchar(4000);
	DECLARE @oldcompanycode NVARCHAR(50);
	DECLARE @oldshowfrom BIT;
	DECLARE @oldshowto BIT;
	DECLARE @oldaddress1 NVARCHAR(250);
	DECLARE @oldaddress2 NVARCHAR(250);
	DECLARE @oldcity NVARCHAR(250);
	DECLARE @oldcounty NVARCHAR(250);
	DECLARE @oldpostcode NVARCHAR(250);
	DECLARE @oldcountry INT;
	DECLARE @oldparentcompanyid INT;
	DECLARE @oldiscompany BIT;
	DECLARE @oldIsPrivateAddress BIT;
	SELECT  @oldcompany = company, @oldcomment = comment, @oldcompanycode = companycode, @oldshowfrom = showfrom, @oldshowto = showto, @oldaddress1 = address1, @oldaddress2 = address2, @oldcity = city, @oldcounty = county, @oldpostcode = postcode, @oldcountry = country, @oldparentcompanyid = parentcompanyid, @oldiscompany = iscompany, @oldIsPrivateAddress = isPrivateAddress FROM companies WHERE companyid = @companyid;

	UPDATE companies SET company = @company, comment = @comment, companycode = @companycode, showfrom = @showfrom, showto = @showto, modifiedby = @userid, modifiedon = @date, address1 = @address1, address2 = @address2, city = @city, county = @county, postcode = @postcode, country = @country, parentcompanyid = @parentcompanyid, iscompany = @iscompany, isPrivateAddress = @isPrivateAddress WHERE companyid = @companyid;
	
	if @userid > 0
	Begin
		if @oldcompany <> @company
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @companyid, 'f31f204a-e714-4f0b-9b3d-7170db58a30b', @oldcompany, @company, @company, null;
		if @oldcomment <> @comment
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @companyid, '9015bb92-a598-4240-ac0a-822b05bd858c', @oldcomment, @comment, @company, null;
		if @oldcompanycode <> @companycode
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @companyid, '258118d7-9b57-4c16-a99a-b7d07edc9a54', @oldcompanycode, @companycode, @company, null;
		if @oldshowfrom <> @showfrom
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @companyid, '3580ac20-cdf0-413d-b77d-7ff5fb748851', @oldshowfrom, @showfrom, @company, null;
		if @oldshowto <> @showto
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @companyid, '105d7186-d488-44a1-9ae7-dea03ec5dd25', @oldshowto, @showto, @company, null;
		if @oldaddress1 <> @address1
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @companyid, '9086176a-dfb9-4151-94dc-ee345f0a594b', @oldaddress1, @address1, @company, null;
		if @oldaddress2 <> @address2
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @companyid, '4a3c1107-4b9f-44ed-8c03-ec01772e1584', @oldaddress2, @address2, @company, null;
		if @oldcity <> @city
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @companyid, '3d633887-6a9d-4d7d-8782-81ea6e1cf1ba', @oldcity, @city, @company, null;
		if @oldcounty <> @county
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @companyid, 'af280d90-97e6-4590-9241-6364d8bde4ef', @oldcounty, @county, @company, null;
		if @oldpostcode <> @postcode
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @companyid, 'b84c65c7-7140-4930-b90e-ca5ad9374017', @oldpostcode, @postcode, @company, null;
		if @oldcountry <> @country
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @companyid, '488ad114-e79d-43ca-a849-de68035b9c64', @oldcountry, @country, @company, null;
		if @oldparentcompanyid <> @parentcompanyid
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @companyid, '8d3a1070-041c-41d0-b737-707992d01132', @oldparentcompanyid, @parentcompanyid, @company, null;
		if @oldiscompany <> @iscompany
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @companyid, 'b524c3d5-0c09-4947-9b48-b25c958d9c75', @oldiscompany, @iscompany, @company, null;
		if @oldIsPrivateAddress <> @isPrivateAddress
			exec addUpdateEntryToAuditLog @userid, @delegateID, 38, @companyid, 'NEWFIELDID', @oldIsPrivateAddress, @isPrivateAddress, @company, null;
	end
end

return @companyid



 

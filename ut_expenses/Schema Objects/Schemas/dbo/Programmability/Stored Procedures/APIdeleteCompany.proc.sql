CREATE PROCEDURE [dbo].[APIdeleteCompany]
	@companyid int
	
AS
	IF EXISTS (SELECT * FROM CompanyEsrAllocation WHERE companyid = @companyid AND (ESRLocationID IS NOT NULL OR ESRAddressID IS NOT NULL))
	BEGIN
		UPDATE companies SET archived = 1 WHERE companyid = @companyid
	END
	ELSE
	BEGIN
		DELETE FROM companies WHERE [companyid] = @companyid;
	END
RETURN 0
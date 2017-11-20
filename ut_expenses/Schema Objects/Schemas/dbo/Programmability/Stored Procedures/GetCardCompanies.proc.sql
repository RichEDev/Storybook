
CREATE PROCEDURE [dbo].[GetCardCompanies] 
AS
BEGIN
	SELECT cardCompanyID, companyName, companyNumber, usedForImport, createdOn, createdBy, modifiedOn, modifiedBy FROM cardCompanies
END

CREATE VIEW [dbo].[fromcompanies]
AS
SELECT     companyid AS fromid, company AS fromcompany, archived, comment, companycode AS code
FROM         dbo.companies

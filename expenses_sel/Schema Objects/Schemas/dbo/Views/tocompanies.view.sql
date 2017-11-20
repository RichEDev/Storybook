CREATE VIEW [dbo].[tocompanies]
AS
SELECT     companyid AS toid, company AS tocompany, archived, comment, companycode AS code
FROM         dbo.companies

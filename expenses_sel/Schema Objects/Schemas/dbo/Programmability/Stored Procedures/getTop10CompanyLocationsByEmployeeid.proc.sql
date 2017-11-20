-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[getTop10CompanyLocationsByEmployeeid] (@employeeid INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT companyid, company
	FROM companies
		
		WHERE companyid IN (SELECT TOP 10 companies.companyid FROM [savedexpenses]
		INNER JOIN companies ON companies.companyid = savedexpenses.companyid
		INNER JOIN claims ON claims.claimid = savedexpenses.[claimid] WHERE claims.employeeid = @employeeid GROUP BY companies.companyid ORDER BY COUNT(*) desc)
		AND iscompany = 1
		ORDER BY company
END
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[getLast10CompanyLocationsByEmployeeid] (@employeeid INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT companyid, company
	FROM companies
		
		WHERE companyid IN (SELECT TOP 10 companyid FROM [savedexpenses]
		
		
		INNER JOIN claims ON claims.claimid = savedexpenses.[claimid] WHERE claims.employeeid = @employeeid ORDER BY date desc)
		AND iscompany = 1
		ORDER BY company
END

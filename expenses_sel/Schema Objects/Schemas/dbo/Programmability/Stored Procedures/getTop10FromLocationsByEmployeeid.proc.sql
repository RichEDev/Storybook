-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[getTop10FromLocationsByEmployeeid] (@employeeid INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT companyid, company
	FROM companies
		
		WHERE companyid IN (SELECT TOP 10 start_location FROM [savedexpenses_journey_steps]
		INNER JOIN [companies] ON [savedexpenses_journey_steps].[start_location] = [companies].[companyid]
		INNER JOIN savedexpenses ON savedexpenses.expenseid = [savedexpenses_journey_steps].[expenseid]
		INNER JOIN claims ON claims.claimid = savedexpenses.[claimid] WHERE claims.employeeid = @employeeid GROUP BY start_location ORDER BY COUNT(*) desc)
		AND showfrom = 1
		ORDER BY company
END

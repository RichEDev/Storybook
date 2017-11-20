-- Stored Procedure

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE saveClaimantFlagJustification 
	@flaggedItemId int,
	@claimantJustification nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    update savedexpensesFlags set claimantJustification = @claimantJustification where flaggedItemId = @flaggedItemId

END
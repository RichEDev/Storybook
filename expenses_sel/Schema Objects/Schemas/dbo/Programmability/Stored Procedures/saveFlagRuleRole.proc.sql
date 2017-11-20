CREATE PROCEDURE [dbo].[saveFlagRuleRole]
	-- Add the parameters for the stored procedure here
	@flagID INT,
	@roleID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO dbo.flagAssociatedRoles ( flagID, roleID ) VALUES  (@flagID, @roleID)
END

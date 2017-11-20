CREATE PROCEDURE [dbo].[saveFlagRuleField]
	-- Add the parameters for the stored procedure here
	@flagID INT,
	@fieldID UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO dbo.flagAssociatedFields ( flagID, fieldID ) VALUES  (@flagID, @fieldID)
END

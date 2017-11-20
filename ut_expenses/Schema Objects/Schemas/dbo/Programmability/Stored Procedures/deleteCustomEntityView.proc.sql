CREATE PROCEDURE [dbo].[deleteCustomEntityView] (@viewid INT,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- System views cannot be deleted
	IF EXISTS (SELECT * FROM [customEntityViews] WHERE viewid = @viewid AND BuiltIn = 1)
		RETURN -2;

	BEGIN TRY 
		DELETE FROM [customEntityViews] WHERE viewid = @viewid;
		return 0;
	END TRY
	BEGIN CATCH
		return -1;
	END CATCH
END



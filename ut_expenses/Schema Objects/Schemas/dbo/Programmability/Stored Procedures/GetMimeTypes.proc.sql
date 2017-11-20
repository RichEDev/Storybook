CREATE PROCEDURE [dbo].[GetMimeTypes] 
	@SubAccountID int
AS
BEGIN
	select mimeID, globalMimeID, archived, createdOn, createdBy, modifiedOn, modifiedBy from dbo.mimeTypes where SubAccountID = @SubAccountID
END

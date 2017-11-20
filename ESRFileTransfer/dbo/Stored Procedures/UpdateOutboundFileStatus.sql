CREATE PROCEDURE [dbo].[UpdateOutboundFileStatus]
      
@DataID int,
@Status tinyint
AS
BEGIN
      UPDATE OutboundFileData SET Status = @Status WHERE DataID = @DataID;
END

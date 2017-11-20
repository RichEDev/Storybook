CREATE PROCEDURE [dbo].[UpdateInboundFileStatus]
      
@DataID int,
@Status tinyint
AS
BEGIN
      UPDATE InboundFileData SET Status = @Status WHERE DataID = @DataID;
END

CREATE PROCEDURE [dbo].[DeleteLogonMessages]
@messageId INT
AS

BEGIN
	SET NOCOUNT ON;
	DELETE FROM MessageModuleBase where MessageId = @messageId;
	DELETE FROM LogonMessages WHERE MessageId = @messageId;
	END
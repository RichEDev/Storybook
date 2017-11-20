
CREATE VIEW [dbo].[LogonMessagesView]
AS
SELECT 
    MessageId
	,Archived
	,CategoryTitle
	,HeaderText FROM [$(targetMetabase)].dbo.LogonMessages
GO





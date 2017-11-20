
CREATE FUNCTION [dbo].[GetModulesForMessage] (@messageId AS INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @ModuleString NVARCHAR(MAX)

	SELECT DISTINCT @ModuleString = COALESCE(@ModuleString + ', ', '') + Cast(mb.moduleName AS VARCHAR) 
	FROM [$(targetMetabase)].dbo.MessageModuleBase MMB INNER JOIN [$(targetMetabase)].dbo.moduleBase MB ON MB.moduleID = mmb.ModuleId WHERE MMB.MessageId = @messageId

	RETURN @ModuleString
END


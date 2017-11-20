



CREATE PROCEDURE [dbo].[entitiesInWorkflow]
	@workflowid int
AS 
	DECLARE @count INT;
	SET @count = (SELECT COUNT(*) FROM workflowEntityState WHERE workflowID=@workflowid);

RETURN @count;



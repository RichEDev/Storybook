-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[clearAuditLog]
@employeeid INT,
@delegateid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM dbo.auditLog
	
	EXEC addDeleteEntryToAuditLog @employeeid, @delegateid, 9, null, 'Audit log cleared', null
END

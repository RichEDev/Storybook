CREATE PROCEDURE [dbo].[deleteReason]
@reasonid int,
@employeeID INT,
@delegateID INT
AS
declare @tmpCount INT
	SET @tmpCount = (select count(reasonid) from savedexpenses where reasonid = @reasonid)
	IF @tmpCount > 0
		RETURN 1;
	declare @returnVal int = 0;
 	declare @tableId uniqueidentifier = (select tableid from tables where tablename = 'reasons');
	exec @returnVal = dbo.checkReferencedBy @tableId, @reasonid;
	if @returnVal = 0
	begin
		declare @recordTitle nvarchar(2000);
		select @recordTitle = reason from reasons where reasonid = @reasonid;

		update savedexpenses set reasonid = null where reasonid = @reasonid;
		update savedexpenses_previous set reasonid = null where reasonid = @reasonid;
		delete from reasons where reasonid = @reasonid;

		exec addDeleteEntryToAuditLog @employeeID, @delegateID, 46, @reasonid, @recordTitle, null;
	end
return @returnVal;

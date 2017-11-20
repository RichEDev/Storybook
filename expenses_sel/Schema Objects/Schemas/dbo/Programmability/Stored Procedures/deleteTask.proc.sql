CREATE procedure [dbo].[deleteTask]
@taskId int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @recordTitle nvarchar(2000);
	declare @subAccountId int;
	select @recordTitle = [subject], @subAccountId = subAccountId from tasks where taskId = @taskId;

	delete from tasks where taskId = @taskId;
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @taskId, @recordTitle, @subAccountId;
end




CREATE PROCEDURE [dbo].[deleteImportHistory]
@historyId int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @logId int
	set @logId = (select logId from importHistory where historyId = @historyId)

	declare @title1 nvarchar(500);
	select @title1 = importid from importHistory where historyId = @historyId;

	delete from dbo.logNames where logID = @logId;

	delete from importHistory where historyId = @historyId;

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'History for Import - ' + CAST(@title1 AS nvarchar(20)));

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 36, @historyId, @recordTitle, null;
end




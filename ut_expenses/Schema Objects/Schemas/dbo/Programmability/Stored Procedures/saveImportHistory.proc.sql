



CREATE procedure [dbo].[saveImportHistory]
@historyId int,
@importId int,
@logId int,
@importDate datetime,
@status int,
@appType int,
@dataId int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @count int
	declare @retId int

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'History for Import - ' + CAST(@importId AS nvarchar(20)));

	if @historyId = 0
	begin
		insert into importHistory (importId, logId, importedDate, importStatus, applicationType, dataId, createdOn)
		values (@importId, @logId, @importDate, @status, @appType, @dataId, getdate());

		set @retId = scope_identity();

		if @CUemployeeID > 0
		Begin
			exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 36, @retId, @recordTitle, null;
		end	
	end
	else
	begin
		declare @oldimportId int;
		declare @oldlogId int;
		declare @oldimportDate datetime;
		declare @oldstatus int;
		declare @oldappType int;
		declare @olddataId int;
		select @oldimportId = importId, @oldlogId = logId, @oldimportDate = importedDate, @oldstatus = importStatus, @oldappType = applicationType, @olddataId = dataId from importHistory where historyId = @historyId;

		update importHistory set importId = @importId, logId = @logId, importedDate = @importDate, importStatus = @status, applicationType = @appType, dataId = @dataId, modifiedOn = getdate()
		where historyId = @historyId;

		if @CUemployeeID > 0
		Begin
			if @oldimportId <> @importId
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 36, @historyId, '9ce9c2c0-9add-4794-b86e-263dfa4c08e6', @oldimportId, @importId, @recordtitle, null;
			if @oldlogId <> @logId
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 36, @historyId, 'b422d1d9-1484-445a-a048-7e58efbcc397', @oldlogId, @logId, @recordtitle, null;
			if @oldimportDate <> @importDate
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 36, @historyId, '6c796d7c-80a2-403a-a915-e012bd9e2ac3', @oldimportDate, @importDate, @recordtitle, null;
			if @oldstatus <> @status
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 36, @historyId, '3406c00c-5a6b-4e42-9f6a-d0aed9554669', @oldstatus, @status, @recordtitle, null;
			if @oldappType <> @appType
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 36, @historyId, 'b0829007-6aad-47c2-a585-12c5ca8b5fd3', @oldappType, @appType, @recordtitle, null;
			if @olddataId <> @dataId
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 36, @historyId, '9e243a38-55b7-4b14-a1f9-16d14f34484c', @olddataId, @dataId, @recordtitle, null;
		end
		set @retId = @historyId;
	end

	return @retId;
end






 

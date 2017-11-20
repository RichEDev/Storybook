


CREATE PROCEDURE [dbo].[saveReason]
@reasonid int,
@reason nvarchar(50),
@description nvarchar(4000),
@accountcodevat nvarchar (50),
@accountcodenovat nvarchar (50),
@date DateTime,
@userid int,
@employeeID INT,
@delegateID INT

AS

declare @count int
if (@reasonid = 0)
begin
	set @count = (select count(*) from reasons where reason = @reason);
	if @count > 0
		return -1;

	insert into reasons (reason, [description], accountcodevat, accountcodenovat, createdby, createdon) values (@reason, @description, @accountcodevat, @accountcodenovat, @userid, @date);
	set @reasonid = scope_identity();
	
	if @employeeID > 0
	BEGIN
		exec addInsertEntryToAuditLog @employeeID, @delegateID, 46, @reasonid, @reason, null;
	END
end
else
begin
	set @count = (select count(*) from reasons where reason = @reason and reasonid <> @reasonid);
	if @count > 0
		return -1;

	declare @oldreason nvarchar(50);
	declare @olddescription nvarchar(4000);
	declare @oldaccountcodevat nvarchar(50);
	declare @oldaccountcodenovat nvarchar(50);
	select @oldreason = reason, @olddescription = description, @oldaccountcodevat = accountcodevat, @oldaccountcodenovat = accountcodenovat from reasons where reasonid = @reasonid;

	update reasons set reason = @reason, description = @description, accountcodevat = @accountcodevat, accountcodenovat = @accountcodenovat, modifiedby = @userid, modifiedon = @date where reasonid = @reasonid;
				
	if @employeeID > 0
	BEGIN
		if @oldreason <> @reason
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 46, @reasonid, 'af839fe7-8a52-4bd1-962c-8a87f22d4a10', @oldreason, @reason, @reason, null;
		if @olddescription <> @description
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 46, @reasonid, '44ab09e8-3294-4fdd-ba50-97f5d69c0c64', @olddescription, @description, @reason, null;
		if @oldaccountcodevat <> @accountcodevat
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 46, @reasonid, '71864488-74f0-4990-b6a5-736bbfb5a1bb', @oldaccountcodevat, @accountcodevat, @reason, null;
		if @oldaccountcodenovat <> @accountcodenovat
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 46, @reasonid, '1c15034e-ae99-474f-a8db-2b593fcaea2f', @oldaccountcodenovat, @accountcodenovat, @reason, null;
	END
end

return @reasonid



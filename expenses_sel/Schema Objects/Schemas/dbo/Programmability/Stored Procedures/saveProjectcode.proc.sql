CREATE PROCEDURE [dbo].[saveProjectcode]
@projectcodeid int,
@projectcode nvarchar(50),
@description nvarchar(2000),
@rechargeable BIT,
@date DateTime,
@userid int,
@CUemployeeID INT,
@CUdelegateID INT
AS

DECLARE @count INT;
declare @usingdescription nvarchar(10)
select @usingdescription = stringValue from accountProperties where stringKey = 'useProjectCodeDescription' and subAccountID in (select top 1 subAccountID from accountsSubAccounts)
if @projectcodeid = 0
BEGIN
	SET @count = (SELECT COUNT(*) FROM project_codes WHERE [projectcode] = @projectcode);
	IF @count > 0
		RETURN -1;
		
	if @usingdescription = '1'
		begin
			set @count = (select count(*) from project_codes where description = @description)
			if @count > 0
				return -2
				
		end
		INSERT INTO project_codes (projectcode, [description], [rechargeable], createdon, createdby) VALUES (@projectcode, @description, @rechargeable, @date, @userid);
		set @projectcodeid = scope_identity()
		
		if @CUemployeeID > 0
		BEGIN
			EXEC addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 3, @projectcodeid, @projectcode, null;
		END
end
else
BEGIN
	SET @count = (SELECT COUNT(*) FROM project_codes WHERE projectcode = @projectcode AND projectcodeid <> @projectcodeid);
	if @count > 0
		return -1

if @usingdescription = '1'
		begin
			set @count = (select count(*) from project_codes where description = @description and projectcodeid <> @projectcodeid)
			if @count > 0
				return -2
				
		end
	declare @oldprojectcode nvarchar(50);
	declare @olddescription nvarchar(2000);
	declare @oldrechargeable BIT;
	select @oldprojectcode = projectcode, @olddescription = [description], @oldrechargeable = rechargeable from project_codes where [projectcodeid] = @projectcodeid;

	UPDATE project_codes SET projectcode = @projectcode, [description] = @description, rechargeable = @rechargeable, [ModifiedOn] = @date, modifiedby = @userid WHERE [projectcodeid] = @projectcodeid;

	if @CUemployeeID > 0
	BEGIN
		if @oldprojectcode <> @projectcode
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 3, @projectcodeid, '6d06b15e-a157-4f56-9ff2-e488d7647219', @oldprojectcode, @projectcode, @projectcode, null;
		if @olddescription <> @description
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 3, @projectcodeid, '0ad6004f-7dfd-4655-95fe-5c86ff5e4be4', @olddescription, @description, @projectcode, null;
		if @oldrechargeable <> @rechargeable
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 3, @projectcodeid, '2396564b-76a4-4c0c-971b-1e7915e9f3a0', @oldrechargeable, @rechargeable, @projectcode, null;
	END
end

return @projectcodeid

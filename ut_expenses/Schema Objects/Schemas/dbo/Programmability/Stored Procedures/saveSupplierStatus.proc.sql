CREATE PROCEDURE [dbo].[saveSupplierStatus]
(
@ID int,
@subAccountId int,
@description nvarchar(50),
@sequence smallint,
@deny_contract_add bit,
@employeeId int,
@delegateID int
)
AS
BEGIN
declare @StatusID int;
declare @count int;
DECLARE @recordTitle nvarchar(2000);

	if @ID = -1
	begin
		set @count = (SELECT COUNT(*) FROM supplier_status where description = @description and subaccountid = @subAccountId)
		if @count > 0
			return -1;

		insert into supplier_status (subaccountid, description, sequence, deny_contract_add, createdon, createdby, modifiedon, modifiedby)
		values (@subAccountId, @description, @sequence, @deny_contract_add, getdate(), @employeeId, null, null)

		set @StatusID = scope_identity()

		set @recordTitle = 'Supplier Status ID: ' + CAST(@StatusID AS nvarchar(20)) + ' (' + @description + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 55, @StatusID, @recordTitle, @subAccountId;
	end
	else
	begin
		set @count = (SELECT COUNT(*) FROM supplier_status where description = @description and subaccountid = @subaccountid and statusid <> @ID)
		if @count > 0
			return -1;

		DECLARE @olddescription nvarchar(50);
		DECLARE @oldsequence smallint;
		DECLARE @olddeny_contract_add bit;
		DECLARE @olddenycontractadd varchar(5) = 'No';
		DECLARE @denycontractadd varchar(5) = 'No';

		select @olddescription = description, @oldsequence = Sequence, @olddeny_contract_add = deny_contract_add from supplier_status where statusid = @ID;

		update supplier_status set
		description = @description,
		sequence = @sequence,
		deny_contract_add = @deny_contract_add,
		modifiedon = getutcdate(),
		modifiedby = @employeeId
		where statusid = @ID

		set @StatusID = @ID

		set @recordTitle = 'Supplier Status ID: ' + CAST(@ID AS nvarchar(20)) + ' (' + @description + ')';
		IF @olddeny_contract_add = 1 set @olddenycontractadd = 'Yes';
		IF @deny_contract_add = 1 set @denycontractadd = 'Yes';

		if @olddescription <> @description
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 55, @ID, 'D66C488C-CF20-44C2-AD94-0EA060553DDB', @olddescription, @description, @recordtitle, @subAccountId;
		if @olddeny_contract_add <> @deny_contract_add
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 55, @ID, 'C57FB904-2AC9-44AB-BA81-723CD3DA2993',  @olddenycontractadd, @denycontractadd, @recordtitle, @subAccountId;
		if @oldsequence <> @sequence
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 55, @ID, '8E97B5CA-2B39-40AD-BCF6-C638F7787A9A', @oldsequence, @sequence, @recordtitle, @subAccountId;
	end

	return @StatusID
END
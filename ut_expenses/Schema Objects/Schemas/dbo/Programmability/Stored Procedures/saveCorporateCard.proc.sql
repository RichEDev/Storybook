

CREATE PROCEDURE [dbo].[saveCorporateCard]
@cardproviderid int,
@claimantsettlesbill BIT,
@allocateditem INT,
@blockcash BIT,
@reconciledbyadministrator BIT,
@singleclaim BIT,
@blockunmatched BIT,
@date DateTime,
@userid int,
@delegateID INT,
@fileIdentifier NVARCHAR(MAX)

AS

DECLARE @count INT;
SET @count = (SELECT COUNT(*) FROM corporate_cards WHERE cardproviderid = @cardproviderid);

if (@count = 0)
begin
	insert into corporate_cards ([cardproviderid], claimants_settle_bill, createdby, createdon, allocateditem, blockcash, reconciled_by_admin, singleclaim, blockunmatched, FileIdentifier) values (@cardproviderid, @claimantsettlesbill, @userid, @date, @allocateditem, @blockcash, @reconciledbyadministrator, @singleclaim, @blockunmatched, @fileIdentifier)
	
	if @userid > 0
	BEGIN
		exec addInsertEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, @cardproviderid, null;
	END
end
else
begin
	declare @oldcardproviderid int;
	declare @oldclaimantsettlesbill BIT;
	declare @oldallocateditem INT;
	declare @oldblockcash BIT;
	declare @oldreconciledbyadministrator BIT;
	declare @oldsingleclaim BIT;
	declare @oldblockunmatched BIT;
	select @oldcardproviderid = [cardproviderid], @oldclaimantsettlesbill = claimants_settle_bill, @oldallocateditem = allocateditem, @oldblockcash = blockcash, @oldreconciledbyadministrator = reconciled_by_admin, @oldsingleclaim = singleclaim, @oldblockunmatched = blockunmatched from corporate_cards where cardproviderid = @cardproviderid;

	update corporate_cards set [cardproviderid] = @cardproviderid, claimants_settle_bill = @claimantsettlesbill, modifiedby = @userid, modifiedon = @date, allocateditem = @allocateditem, blockcash = @blockcash, reconciled_by_admin = @reconciledbyadministrator, singleclaim = @singleclaim, blockunmatched = @blockunmatched, FileIdentifier = @fileIdentifier where cardproviderid = @cardproviderid;
	
	if @userid > 0
	BEGIN
		if @oldcardproviderid <> @cardproviderid
			exec addUpdateEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, null, @oldcardproviderid, @cardproviderid, @cardproviderid, null;
		if @oldclaimantsettlesbill <> @claimantsettlesbill
			exec addUpdateEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, null, @oldclaimantsettlesbill, @claimantsettlesbill, @cardproviderid, null;
		if @oldallocateditem <> @allocateditem
			exec addUpdateEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, null, @oldallocateditem, @allocateditem, @cardproviderid, null;
		if @oldblockcash <> @blockcash
			exec addUpdateEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, null, @oldblockcash, @blockcash, @cardproviderid, null;
		if @oldreconciledbyadministrator <> @reconciledbyadministrator
			exec addUpdateEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, null, @oldreconciledbyadministrator, @reconciledbyadministrator, @cardproviderid, null;
		if @oldsingleclaim <> @singleclaim
			exec addUpdateEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, null, @oldsingleclaim, @singleclaim, @cardproviderid, null;
		if @oldblockunmatched <> @blockunmatched
			exec addUpdateEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, null, @oldblockunmatched, @blockunmatched, @cardproviderid, null;
	END
end

return 0





 

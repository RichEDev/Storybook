CREATE PROCEDURE [dbo].[saveCorporateCard]
@cardproviderid INT,
@claimantsettlesbill BIT,
@allocateditem INT,
@blockcash BIT,
@reconciledbyadministrator BIT,
@singleclaim BIT,
@date DATETIME,
@userid INT,
@delegateID INT,
@fileIdentifier NVARCHAR(MAX)

AS

DECLARE @count INT;
SET @count = (SELECT COUNT(*) FROM corporate_cards WHERE cardproviderid = @cardproviderid);

IF (@count = 0)
BEGIN
	INSERT INTO corporate_cards ([cardproviderid], claimants_settle_bill, createdby, createdon, allocateditem, blockcash, reconciled_by_admin, singleclaim, FileIdentifier) VALUES (@cardproviderid, @claimantsettlesbill, @userid, @date, @allocateditem, @blockcash, @reconciledbyadministrator, @singleclaim, @fileIdentifier)
	
	IF @userid > 0
	BEGIN
		EXEC addInsertEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, @cardproviderid, null;
	END
END
ELSE
BEGIN
	DECLARE @oldcardproviderid INT;
	DECLARE @oldclaimantsettlesbill BIT;
	DECLARE @oldallocateditem INT;
	DECLARE @oldblockcash BIT;
	DECLARE @oldreconciledbyadministrator BIT;
	DECLARE @oldsingleclaim BIT;
	SELECT @oldcardproviderid = [cardproviderid], @oldclaimantsettlesbill = claimants_settle_bill, @oldallocateditem = allocateditem, @oldblockcash = blockcash, @oldreconciledbyadministrator = reconciled_by_admin, @oldsingleclaim = singleclaim FROM corporate_cards WHERE cardproviderid = @cardproviderid;

	UPDATE corporate_cards SET [cardproviderid] = @cardproviderid, claimants_settle_bill = @claimantsettlesbill, modifiedby = @userid, modifiedon = @date, allocateditem = @allocateditem, blockcash = @blockcash, reconciled_by_admin = @reconciledbyadministrator, singleclaim = @singleclaim, FileIdentifier = @fileIdentifier WHERE cardproviderid = @cardproviderid;
	
	IF @userid > 0
	BEGIN
		IF @oldcardproviderid <> @cardproviderid
			EXEC addUpdateEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, null, @oldcardproviderid, @cardproviderid, @cardproviderid, null;
		IF @oldclaimantsettlesbill <> @claimantsettlesbill
			EXEC addUpdateEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, null, @oldclaimantsettlesbill, @claimantsettlesbill, @cardproviderid, null;
		IF @oldallocateditem <> @allocateditem
			EXEC addUpdateEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, null, @oldallocateditem, @allocateditem, @cardproviderid, null;
		IF @oldblockcash <> @blockcash
			EXEC addUpdateEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, null, @oldblockcash, @blockcash, @cardproviderid, null;
		IF @oldreconciledbyadministrator <> @reconciledbyadministrator
			EXEC addUpdateEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, null, @oldreconciledbyadministrator, @reconciledbyadministrator, @cardproviderid, null;
		IF @oldsingleclaim <> @singleclaim
			EXEC addUpdateEntryToAuditLog @userid, @delegateID, 17, @cardproviderid, null, @oldsingleclaim, @singleclaim, @cardproviderid, null;
	END
END

RETURN 0
 

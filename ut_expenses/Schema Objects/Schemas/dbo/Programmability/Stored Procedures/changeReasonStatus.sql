CREATE PROCEDURE [dbo].[changeReasonStatus]
@reasonid INT,
@archive BIT,
@employeeID INT,
@delegateID INT
AS
DECLARE @recordtitle nvarchar(2000);
DECLARE @oldarchive BIT;
SELECT @recordtitle = reason FROM reasons WHERE reasonid = @reasonid;
SELECT @oldarchive = archived FROM reasons WHERE reasonid = @reasonid;

UPDATE reasons SET archived = @archive, modifiedon = getdate(), modifiedby = @employeeID WHERE reasonid = @reasonid;

IF @oldarchive <> @archive
	EXEC addUpdateEntryToAuditLog @employeeID, @delegateID, 46, @reasonid, 'cdc425fe-9860-4598-95fd-07d22e840255', @oldarchive, @archive, @recordtitle, null;

RETURN 0;

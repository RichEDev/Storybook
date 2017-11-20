





CREATE PROCEDURE [dbo].[saveESRElement]
	
@elementID int,
@globalElementID int,
@NHSTrustID int,
@employeeID INT,
@delegateID INT

AS

declare @recordTitle nvarchar(2000);

if @elementID = 0
BEGIN
	INSERT INTO ESRElements (globalElementID, NHSTrustID) VALUES (@globalElementID, @NHSTrustID);
	SET @elementID = scope_identity();

	set @recordTitle = (select '@NHSTrustID ' + CAST(@NHSTrustID AS nvarchar(20)) + ' - @globalElementID ' + CAST(@globalElementID AS nvarchar(20)));
	exec addInsertEntryToAuditLog @employeeID, @delegateID, 26, @elementID, @recordTitle, null;
END
else
BEGIN

	declare @oldglobalElementID int;
	select @oldglobalElementID = globalElementID from ESRElements WHERE elementID = @elementID;

	UPDATE ESRElements SET globalElementID = @globalElementID WHERE elementID = @elementID
	if @oldglobalElementID <> @globalElementID
		begin
			set @recordTitle = (select '@NHSTrustID ' + CAST(@NHSTrustID AS nvarchar(20)) + ' - @globalElementID ' + CAST(@globalElementID AS nvarchar(20)));
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 26, @elementID, '0d426c2a-9a67-4654-bebc-71d504757b34', @oldglobalElementID, @globalElementID, @recordtitle, null;
		end
END

return @elementID






 

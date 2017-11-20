




CREATE PROCEDURE [dbo].[saveESRElementField]
	
@elementFieldID int,
@elementID int,
@globalElementFieldID int,
@reportColumnID uniqueidentifier,
@aggregate tinyint,
@order tinyint,
@employeeID INT,
@delegateID INT

AS

if @elementFieldID = 0
BEGIN
	INSERT INTO ESRElementFields (elementID, globalElementFieldID, reportColumnID, [aggregate], [order]) VALUES (@elementID, @globalElementFieldID, @reportColumnID, @aggregate, @order)
	SET @elementFieldID = scope_identity()

	exec addInsertEntryToAuditLog @employeeID, @delegateID, 26, @elementFieldID, 'Element Field', null;
END

return @elementFieldID






 

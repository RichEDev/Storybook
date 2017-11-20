CREATE PROCEDURE [dbo].[deleteInvoiceStatusType] 
(
@ID INT, 
@employeeId INT,
@delegateID int
)
AS
DECLARE @description nvarchar(50);
DECLARE @subAccountId int;
DECLARE @cnt int;
SET @cnt = 0;

select @description = description, @subAccountId = subAccountId from invoiceStatusType where invoiceStatusTypeId = @ID;

select @cnt = COUNT(invoiceID) from invoices where invoiceStatus = @ID;

IF @cnt > 0
	BEGIN
		RETURN -1;
	END
	
delete from invoiceStatusType where invoiceStatusTypeId = @ID;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 131, @ID, @description, @subAccountId;

return @cnt

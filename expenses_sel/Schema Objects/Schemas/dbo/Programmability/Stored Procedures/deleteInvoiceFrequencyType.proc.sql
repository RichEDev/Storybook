CREATE PROCEDURE [dbo].[deleteInvoiceFrequencyType] 
(
@ID INT, 
@employeeId INT,
@delegateID int
)
AS
DECLARE @invoiceFrequencyTypeDesc nvarchar(50);
DECLARE @subAccountId int;
DECLARE @cnt int;
SET @cnt = 0;

select @invoiceFrequencyTypeDesc = invoiceFrequencyTypeDesc, @subAccountId = subAccountId from codes_invoicefrequencytype where invoiceFrequencyTypeId = @ID;

select @cnt = COUNT(contractId) from contract_details where invoiceFrequencyTypeId = @ID;

IF @cnt > 0
	BEGIN
		RETURN -1;
	END

delete from codes_invoicefrequencytype where invoiceFrequencyTypeId = @ID;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 130, @ID, @invoiceFrequencyTypeDesc, @subAccountId;

return @cnt

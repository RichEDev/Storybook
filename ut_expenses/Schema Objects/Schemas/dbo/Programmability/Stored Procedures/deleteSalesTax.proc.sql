CREATE PROCEDURE [dbo].[deleteSalesTax] 
(
@ID INT, 
@employeeId INT,
@delegateID int
)
AS
DECLARE @description nvarchar(10);
DECLARE @cnt int;
SET @cnt = 0;
DECLARE @cnt1 int;
DECLARE @cnt2 int;
SET @cnt1 = 0;
SET @cnt2 = 0;

select @description = description from codes_salesTax where salesTaxId = @ID;

select @cnt1 = COUNT(contractId) from contract_productdetails where salesTaxRate = @ID;
select @cnt2 = COUNT(invoiceID) from invoices where salesTaxRate = @ID;
SET @cnt = @cnt1 + @cnt2;

IF @cnt > 0
	BEGIN
		RETURN -1;
	END
	
delete from codes_salesTax where salesTaxId = @ID;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 114, @ID, @description, null;

return @cnt

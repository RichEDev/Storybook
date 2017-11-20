









CREATE PROCEDURE [dbo].[deleteInvoiceLineItemCostCentres]
	@invoiceLineItemId int,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	DELETE FROM invoiceLineItemCostCentres WHERE invoiceLineItemId = @invoiceLineItemId;







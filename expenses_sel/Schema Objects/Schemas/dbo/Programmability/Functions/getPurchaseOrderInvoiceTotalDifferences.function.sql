CREATE FUNCTION [dbo].[getPurchaseOrderInvoiceTotalDifferences](@purchaseOrderID INT, @invoiceID INT) 
RETURNS DECIMAL(18,2)
AS
BEGIN
	DECLARE @total DECIMAL(18,2);
	DECLARE @poTotal DECIMAL(18,2);
	DECLARE @invoiceTotal DECIMAL(18,2);
	SET @poTotal = (SELECT SUM((purchaseOrderProducts.productUnitPrice * purchaseOrderProducts.productQuantity)) FROM purchaseOrderProducts WHERE purchaseOrderProducts.purchaseorderID=@purchaseOrderID);
	SET @invoiceTotal = (SELECT SUM((invoiceLineItems.unitPrice * invoiceLineItems.quantity)) FROM invoiceLineItems INNER JOIN invoices ON invoiceLineItems.invoiceID=invoices.invoiceID WHERE invoices.invoiceID=@invoiceID)

	SET @total = @poTotal - @invoiceTotal;

	RETURN @total;
END








CREATE FUNCTION [dbo].[getInvoiceTotal](@invoiceID INT) 
RETURNS DECIMAL(18,2)
AS
BEGIN
	DECLARE @total DECIMAL(18,2);
	SET @total = (SELECT SUM(invoiceLineItems.unitPrice * invoiceLineItems.quantity) FROM invoiceLineItems WHERE invoiceLineItems.invoiceID=@invoiceID);
	RETURN @total;
END






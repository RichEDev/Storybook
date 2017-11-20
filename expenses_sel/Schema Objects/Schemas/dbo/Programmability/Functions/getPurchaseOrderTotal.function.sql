CREATE FUNCTION dbo.getPurchaseOrderTotal(@purchaseOrderID INT) 
RETURNS DECIMAL(18,2)
AS
BEGIN
	DECLARE @total DECIMAL(18,2);
	SET @total = (SELECT SUM((purchaseOrderProducts.productUnitPrice * purchaseOrderProducts.productQuantity)) FROM purchaseOrderProducts WHERE purchaseOrderProducts.purchaseorderID=@purchaseOrderID);
	RETURN @total;
END

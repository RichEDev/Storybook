



CREATE PROCEDURE [dbo].[savePurchaseOrderProduct]
	@purchaseOrderProductID int,
	@productID int,
	@purchaseOrderID int,
	@unitID int, 
	@unitPrice money,
	@quantity decimal(18,2),
	@employeeID int
AS 
	IF (@purchaseOrderProductID = 0)
		BEGIN
			INSERT INTO purchaseOrderProducts (productID, purchaseOrderID, productUnitOfMeasureID, productUnitPrice, productQuantity) VALUES  (@productID, @purchaseOrderID, @unitID, @unitPrice, @quantity);
			UPDATE purchaseOrders SET modifiedOn=getdate(), modifiedby=@employeeID WHERE purchaseOrderID=@purchaseOrderID;
			SET @purchaseOrderProductID = scope_identity();
		END
	ELSE
		BEGIN
			UPDATE purchaseOrderProducts SET productID=@productID, productUnitOfMeasureID=@unitID, productUnitPrice=@unitPrice, productQuantity=@quantity WHERE purchaseOrderProductID=@purchaseOrderProductID;
			UPDATE purchaseOrders SET modifiedOn=getdate(), modifiedby=@employeeID WHERE purchaseOrderID=@purchaseOrderID;
		END
RETURN @purchaseOrderProductID;












CREATE PROCEDURE [dbo].[deletePurchaseOrderDelivery]
	@purchaseOrderID int,
	@deliveryID int,
	@employeeID int	
AS 
	UPDATE purchaseOrders SET modifiedOn=getdate(), modifiedBy=@employeeID WHERE purchaseOrderID = @purchaseOrderID
	DELETE FROM purchaseOrderDeliveries WHERE deliveryID=@deliveryID




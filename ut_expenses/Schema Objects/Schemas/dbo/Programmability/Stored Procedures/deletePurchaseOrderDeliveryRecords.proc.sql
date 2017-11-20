






CREATE PROCEDURE [dbo].[deletePurchaseOrderDeliveryRecords]
	@purchaseOrderID int,
	@deliveryID int,
	@employeeID int	
AS 
	UPDATE purchaseOrders SET modifiedOn = getdate(), modifiedBy = @employeeID WHERE purchaseOrderID = @purchaseOrderID;
	UPDATE purchaseOrderDeliveries SET modifiedOn = getdate(), modifiedby = @employeeID WHERE deliveryID = @deliveryID;
	DELETE FROM purchaseOrderDeliveryRecords WHERE deliveryID = @deliveryID;





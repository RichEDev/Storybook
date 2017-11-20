





CREATE PROCEDURE [dbo].[savePurchaseOrderDeliveryRecord]
@purchaseOrderDeliveryRecordID int,
@purchaseOrderProductID int,
@deliveryID int,
@deliveredQuantity decimal(18,2),
@returnedQuantity decimal(18,2),
@employeeID int,
@purchaseOrderID int
AS 
	IF (@purchaseOrderDeliveryRecordID = 0)
		BEGIN
			INSERT INTO purchaseOrderDeliveryRecords (purchaseOrderProductID, deliveryID, deliveredQuantity, returnedQuantity) VALUES (@purchaseOrderProductID, @deliveryID, @deliveredQuantity, @returnedQuantity)
			UPDATE purchaseOrders SET modifiedOn=getdate(), modifiedby=@employeeID WHERE purchaseOrderID=@purchaseOrderID;
			UPDATE purchaseOrderDeliveries SET modifiedOn=getdate(), modifiedby=@employeeID WHERE deliveryID=@deliveryID;
			SET @purchaseOrderDeliveryRecordID = scope_identity();
		END
	ELSE
		BEGIN
			UPDATE purchaseOrderDeliveryRecords SET deliveredQuantity=@deliveredQuantity, returnedQuantity=@returnedQuantity WHERE purchaseOrderDeliveryRecordID=@purchaseOrderDeliveryRecordID;
			UPDATE purchaseOrders SET modifiedOn=getdate(), modifiedby=@employeeID WHERE purchaseOrderID=@purchaseOrderID;
			UPDATE purchaseOrderDeliveries SET modifiedOn=getdate(), modifiedby=@employeeID WHERE deliveryID=@deliveryID;
		END
RETURN @purchaseOrderDeliveryRecordID;








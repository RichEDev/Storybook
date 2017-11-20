


CREATE PROCEDURE [dbo].[savePurchaseOrderDelivery]
@deliveryID int,
@locationID int,
@purchaseOrderID int,
@deliveryDate datetime,
@deliveryReference nvarchar(50),
@employeeID int
AS 
	IF (@deliveryID = 0)
		BEGIN
			INSERT INTO purchaseOrderDeliveries (locationID, purchaseOrderID, deliveryDate, deliveryReference, createdOn, createdBy) VALUES (@locationID, @purchaseOrderID, @deliveryDate, @deliveryReference, getDate(), @employeeID)
			UPDATE purchaseOrders SET modifiedOn=getdate(), modifiedby=@employeeID WHERE purchaseOrderID=@purchaseOrderID;
			SET @deliveryID = scope_identity();
		END
	ELSE
		BEGIN
			UPDATE purchaseOrderDeliveries SET locationID=@locationID, purchaseOrderID=@purchaseOrderID, deliveryDate=@deliveryDate, deliveryReference=@deliveryReference, modifiedOn=getDate(), modifiedBy=@employeeID WHERE deliveryID=@deliveryID;
			UPDATE purchaseOrders SET modifiedOn=getdate(), modifiedby=@employeeID WHERE purchaseOrderID=@purchaseOrderID;
		END
RETURN @deliveryID;





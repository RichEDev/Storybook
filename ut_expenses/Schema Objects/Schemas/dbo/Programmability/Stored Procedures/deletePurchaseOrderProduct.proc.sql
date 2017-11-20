




CREATE PROCEDURE [dbo].[deletePurchaseOrderProduct]
	@purchaseOrderProductID int	,
	@employeeID int	
AS 
	UPDATE purchaseOrders SET modifiedOn=getdate(), modifiedBy=@employeeID WHERE purchaseOrderID = (SELECT purchaseOrderID FROM purchaseOrderProducts WHERE purchaseOrderProductID=@purchaseOrderProductID)
	DELETE FROM purchaseOrderProducts WHERE purchaseOrderProductID=@purchaseOrderProductID



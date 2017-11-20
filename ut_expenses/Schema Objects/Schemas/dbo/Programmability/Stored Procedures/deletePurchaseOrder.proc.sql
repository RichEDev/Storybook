





CREATE PROCEDURE [dbo].[deletePurchaseOrder]
	@purchaseOrderID int,
	@employeeID int	
AS 
	DELETE FROM purchaseOrders WHERE purchaseOrderID=@purchaseOrderID




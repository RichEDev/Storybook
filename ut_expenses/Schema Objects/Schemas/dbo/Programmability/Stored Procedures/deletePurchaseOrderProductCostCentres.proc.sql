







CREATE PROCEDURE [dbo].[deletePurchaseOrderProductCostCentres]
	@purchaseOrderProductId int
AS 
	DELETE FROM purchaseOrderProductCostCentres WHERE purchaseOrderProductId = @purchaseOrderProductId;






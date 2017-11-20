




CREATE PROCEDURE [dbo].[savePurchaseOrderProductCostCentre]
	@purchaseOrderProductCostCentreId int,
	@purchaseOrderProductId int,
	@costCodeId int,
	@departmentId int,
	@projectCodeId int,
	@percentUsed int,
	@employeeId int
AS 
	IF (@purchaseOrderProductCostCentreId = 0)
		BEGIN
			INSERT INTO purchaseOrderProductCostCentres (purchaseOrderProductId, departmentId, costCodeId, projectCodeId, percentUsed) VALUES  (@purchaseOrderProductId, @departmentId, @costCodeId, @projectCodeId, @percentUsed);
			--UPDATE purchaseOrderProducts SET modifiedOn=getdate(), modifiedby=@employeeId WHERE purchaseOrderProductId=@purchaseOrderProductId;
			SET @purchaseOrderProductCostCentreId = scope_identity();
		END
	ELSE
		BEGIN
			UPDATE purchaseOrderProductCostCentres SET departmentId=@departmentId, costCodeId=@costCodeId, projectCodeId=@projectCodeId, percentUsed=@percentUsed WHERE purchaseOrderProductCostCentreId=@purchaseOrderProductCostCentreId;
			--UPDATE purchaseOrderProducts SET modifiedOn=getdate(), modifiedby=@employeeId WHERE purchaseOrderProductId=@purchaseOrderProductId;
		END
RETURN @purchaseOrderProductCostCentreId;















CREATE PROCEDURE [dbo].[saveInvoiceLineItemCostCentre]
	@invoiceLineItemCostCentreId int,
	@invoiceLineItemId int,
	@costCodeId int,
	@departmentId int,
	@projectCodeId int,
	@percentUsed int
AS 
	IF (@invoiceLineItemCostCentreId = 0)
		BEGIN
			INSERT INTO invoiceLineItemCostCentres (invoiceLineItemId, departmentId, costCodeId, projectCodeId, percentUsed) VALUES  (@invoiceLineItemId, @departmentId, @costCodeId, @projectCodeId, @percentUsed);
			--UPDATE purchaseOrderProducts SET modifiedOn=getdate(), modifiedby=@employeeId WHERE purchaseOrderProductId=@purchaseOrderProductId;
			SET @invoiceLineItemCostCentreId = scope_identity();
		END
	ELSE
		BEGIN
			UPDATE invoiceLineItemCostCentres SET invoiceLineItemId=@invoiceLineItemId, departmentId=@departmentId, costCodeId=@costCodeId, projectCodeId=@projectCodeId, percentUsed=@percentUsed WHERE invoiceLineItemCostCentreId=@invoiceLineItemCostCentreId;
			--UPDATE purchaseOrderProducts SET modifiedOn=getdate(), modifiedby=@employeeId WHERE purchaseOrderProductId=@purchaseOrderProductId;
		END
RETURN @invoiceLineItemCostCentreId;










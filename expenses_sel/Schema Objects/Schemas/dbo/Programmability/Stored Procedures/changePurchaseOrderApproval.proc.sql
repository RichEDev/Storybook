

CREATE procedure [dbo].[changePurchaseOrderApproval]
@state int,
@purchaseOrderId int,
@employeeid int
AS
declare @dateApproved datetime;
IF (@state = 4)
	BEGIN
		UPDATE purchaseOrders SET purchaseOrderState=@state, modifiedon = getdate(), modifiedby = @employeeid where purchaseOrderID = @purchaseOrderId
	END
ELSE
	BEGIN
		IF (@state = 5)
			BEGIN
				SET @dateApproved = NULL;
			END
		ELSE 
			BEGIN
				SET @dateApproved = getdate();
			END

		UPDATE purchaseOrders SET purchaseOrderState=@state, dateApproved = @dateApproved, modifiedon = getdate(), modifiedby = @employeeid where purchaseOrderID = @purchaseOrderId
	END
return;

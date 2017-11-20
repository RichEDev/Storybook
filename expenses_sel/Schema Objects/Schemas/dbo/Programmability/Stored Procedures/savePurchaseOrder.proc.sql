






CREATE PROCEDURE [dbo].[savePurchaseOrder]

@purchaseOrderID int,
@title nvarchar(250),
@supplierID int,
@parentPurchaseOrderID int,
@purchaseOrderState tinyint,
@purchaseOrderNumber nvarchar(50),
@dateApproved datetime,
@comments nvarchar(max),
@orderType tinyint,
@orderRecurrence tinyint,
@orderStartDate datetime,
@orderEndDate datetime,
@employeeID int,
@countryID int,
@currencyID int,
@createdOn datetime
AS 
	IF (@purchaseOrderID = 0)
		BEGIN
			INSERT INTO purchaseOrders (title, supplierID, countryID, currencyID, parentPurchaseOrderID, purchaseOrderState, purchaseOrderNumber, dateApproved, comments, orderType, orderRecurrence, orderStartDate, orderEndDate, createdOn, createdBy) VALUES (@title, @supplierID, @countryID, @currencyID, @parentPurchaseOrderID, @purchaseOrderState, @purchaseOrderNumber, @dateApproved, @comments, @orderType, @orderRecurrence, @orderStartDate, @orderEndDate, @createdOn, @employeeID);
			SET @purchaseOrderID = scope_identity();
		END
	ELSE
		BEGIN
			UPDATE purchaseOrders SET title=@title, supplierID = @supplierID, countryID = @countryID, currencyID = @currencyID, parentPurchaseOrderID = @parentPurchaseOrderID, purchaseOrderNumber = @purchaseOrderNumber, purchaseOrderState = @purchaseOrderState, dateApproved = @dateApproved, comments = @comments, orderType = @orderType, orderRecurrence = @orderRecurrence, orderStartDate = @orderStartDate, orderEndDate = @orderEndDate, modifiedOn = getdate(), modifiedBy=@employeeID WHERE purchaseOrderID=@purchaseOrderID;
		END
	IF (@parentPurchaseOrderID IS NOT NULL)
		BEGIN
			UPDATE purchaseOrders SET modifiedOn = getdate() WHERE purchaseOrderID = @parentPurchaseOrderID;
		END
RETURN @purchaseOrderID;









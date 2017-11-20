



CREATE  PROCEDURE [dbo].[saveInvoice]

@invoiceID int,
@purchaseOrderID int,
@contractID int,
@supplierID int,
@invoiceNumber nvarchar(200),
@receivedDate datetime,
@dueDate datetime,
@invoiceStatus int,
@comment text,
@paidDate datetime,
@coverPeriodEnd datetime,
@employeeID int,
@invoiceState int
AS 
	IF (@invoiceID = 0)
		BEGIN
			INSERT INTO invoices (purchaseOrderID, contractID, supplierID, invoiceNumber, receivedDate, dueDate, invoiceStatus, comment, paidDate, coverPeriodEnd, createdOn, createdBy, invoiceState) VALUES (@purchaseOrderID, @contractID, @supplierID, @invoiceNumber, @receivedDate, @dueDate, @invoiceStatus, @comment, @paidDate, @coverPeriodEnd, getdate(), @employeeID, @invoiceState);
			SET @invoiceID = scope_identity();
		END
	ELSE
		BEGIN
			UPDATE invoices SET purchaseOrderID=@purchaseOrderID, contractID=@contractID, supplierID=@supplierID, invoiceNumber=@invoiceNumber, receivedDate=@receivedDate, dueDate=@dueDate, invoiceStatus=@invoiceStatus, comment=@comment, paidDate=@paidDate, coverPeriodEnd=@coverPeriodEnd, modifiedOn=getdate(), modifiedBy=@employeeID, invoiceState=@invoiceState WHERE invoiceID=@invoiceID
		END
RETURN @invoiceID;




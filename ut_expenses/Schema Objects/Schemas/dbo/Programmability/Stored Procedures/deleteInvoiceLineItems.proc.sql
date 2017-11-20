







CREATE PROCEDURE [dbo].[deleteInvoiceLineItems]
	@invoiceID int,
	@employeeID int,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	UPDATE invoices SET modifiedOn=getdate(), modifiedBy=@employeeID WHERE invoiceID=@invoiceID
	DELETE FROM invoiceLineItems WHERE invoiceID=@invoiceID






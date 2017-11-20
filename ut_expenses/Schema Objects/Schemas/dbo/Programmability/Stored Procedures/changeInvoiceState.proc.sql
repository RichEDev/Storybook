





CREATE procedure [dbo].[changeInvoiceState]
@state int,
@invoiceId int,
@employeeid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	UPDATE invoices SET invoiceState=@state, modifiedon = getdate(), modifiedby = @employeeid where invoiceID = @invoiceId
END
return;





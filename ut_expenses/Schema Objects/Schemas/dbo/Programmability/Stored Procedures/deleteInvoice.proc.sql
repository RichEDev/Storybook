

CREATE PROCEDURE [dbo].[deleteInvoice] (@invoiceID INT,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	DELETE FROM invoices WHERE invoiceID=@invoiceID;
END


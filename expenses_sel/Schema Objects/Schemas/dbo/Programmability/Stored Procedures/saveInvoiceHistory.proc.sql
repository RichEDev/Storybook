CREATE PROCEDURE [dbo].[saveInvoiceHistory]
	@invoiceID INT,
	@comment NVARCHAR(MAX),
	@createdByString NVARCHAR(150),
	@createdOn DATETIME,
	@createdBy int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO invoiceHistory (invoiceid, comment, createdByString, createdon, createdby) VALUES (@invoiceID, @comment, @createdByString, @createdOn, @createdBy)
END

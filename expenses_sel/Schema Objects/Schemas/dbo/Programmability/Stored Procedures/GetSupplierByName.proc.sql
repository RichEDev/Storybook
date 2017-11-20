CREATE PROCEDURE GetSupplierByName 
	-- Add the parameters for the stored procedure here
	@supplierName nvarchar(MAX), 
	@subAccountID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT supplierid FROM supplier_details WHERE suppliername = @supplierName AND subaccountid = @subAccountID
END

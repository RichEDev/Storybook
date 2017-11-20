CREATE PROCEDURE UpdateSupplierCacheExpiry 
	-- Add the parameters for the stored procedure here
	@supplierID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE supplier_details SET CacheExpiry = getdate() where supplierId = @supplierID
END

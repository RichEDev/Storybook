CREATE PROCEDURE DeleteUserDefinedSupplierDetails 
	-- Add the parameters for the stored procedure here
	@supplierID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	delete from userdefinedSupplierDetails where supplierid = @supplierID
END

CREATE PROCEDURE GetFlagAssociatedExpenseItems 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select flagID, flagAssociatedExpenseItems.subcatID, subcat from flagAssociatedExpenseItems inner join subcats on subcats.subcatid = flagAssociatedExpenseItems.subcatid order by flagid, subcats.subcat
END
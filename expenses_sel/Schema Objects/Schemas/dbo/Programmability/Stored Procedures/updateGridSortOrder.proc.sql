

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[updateGridSortOrder] (@employeeid INT, @gridid NVARCHAR(250), @sortedColumn uniqueidentifier, @sortorder TINYINT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @count INT
	
	SET @count = (SELECT COUNT(*) FROM employeeGridSortOrders WHERE employeeid = @employeeid AND gridid = @gridid);
    
	IF @count = 0
		begin
			INSERT INTO employeeGridSortOrders (employeeid, gridid, sortedcolumn, sortorder) VALUES (@employeeid, @gridid, @sortedcolumn, @sortorder);
		END
	ELSE
		BEGIN
			UPDATE employeeGridSortOrders SET sortedcolumn = @sortedcolumn, sortorder = @sortorder WHERE employeeid = @employeeid AND gridid = @gridid
		END
	
END



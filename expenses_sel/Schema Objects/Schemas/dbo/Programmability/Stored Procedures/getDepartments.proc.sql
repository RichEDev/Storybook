
--Create getDepartments sp
CREATE PROCEDURE [dbo].[getDepartments] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	select * from departments order by department
END

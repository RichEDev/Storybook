CREATE PROCEDURE [dbo].[APIdeleteCar]
	@carid int 
	
AS
BEGIN
	DELETE FROM cars WHERE carid = @carid
END
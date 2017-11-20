CREATE PROCEDURE [dbo].[saveEmployeeMenuFavourite]
@menuTitle NVARCHAR(100),
@iconLocation NVARCHAR(150),
@onclickUrl NVARCHAR(250),
@employeeID INT,
@order TINYINT
AS
BEGIN
	DECLARE @count INT;
	DECLARE @retVal INT;

	SET @count = (SELECT COUNT(Title) FROM [employeeMenuFavourites] WHERE EmployeeID = @employeeID AND Title = @menuTitle 
																		AND IconLocation = @iconLocation AND OnClickUrl = @onclickUrl);
	IF @count = 0
	BEGIN
		INSERT INTO employeeMenuFavourites(EmployeeID, Title, IconLocation, OnClickUrl, [Order]) 
		VALUES (@employeeID, @menuTitle, @iconLocation, @onclickUrl, @order);
		SET @retVal = scope_identity();		
	END
	ELSE
		SET @retVal = -1;
				
	RETURN @retVal
END
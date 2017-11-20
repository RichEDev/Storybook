CREATE PROCEDURE [dbo].[deleteEmployeeMenuFavourite]
@menuFavouriteID INT,
@employeeID INT
AS
BEGIN
      DECLARE @count INT;
      DECLARE @retVal INT;

      SET @count = (SELECT COUNT(MenuFavouriteID) FROM [employeeMenuFavourites] WHERE EmployeeID = @employeeID AND MenuFavouriteID = @menuFavouriteID);
      IF @count = 0
		SET @retVal = -1;
      ELSE
      BEGIN
            DELETE FROM employeeMenuFavourites
            WHERE EmployeeID = @employeeID AND MenuFavouriteID = @menuFavouriteID;
            SET @count = (SELECT COUNT(MenuFavouriteID) FROM [employeeMenuFavourites] WHERE EmployeeID = @employeeID AND MenuFavouriteID = @menuFavouriteID);
            
            IF @count = 0
                  BEGIN
                  -- Go through each of the menufavourites for the employee and re-order them. This sorts out ordering issues for later
                        DECLARE @currentId INT;
                        DECLARE @index INT;
                        SET @index = 0;

                        DECLARE MenuFavourites CURSOR
                        FOR SELECT [MenuFavouriteID] FROM [employeeMenuFavourites] WHERE [EmployeeID] = @employeeID ORDER BY [Order]
                        OPEN MenuFavourites
                  
                        FETCH NEXT FROM MenuFavourites INTO @currentId
                        
                        WHILE (@@FETCH_STATUS = 0)
                        BEGIN
                              
                              UPDATE [employeeMenuFavourites] SET [Order] = @index WHERE [MenuFavouriteID] = @currentId;
                              
                              SET @index = @index + 1;
                              
                              FETCH NEXT FROM MenuFavourites INTO @currentId
                              
                        END
                  
                        CLOSE MenuFavourites;
                        DEALLOCATE MenuFavourites;
                        
                        SET @retVal = 1;
                  END
            ELSE
				SET @retVal = -2;
      END   
                        
      RETURN @retVal;
END

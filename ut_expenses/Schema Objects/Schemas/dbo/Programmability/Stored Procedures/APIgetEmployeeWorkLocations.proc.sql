CREATE PROCEDURE [dbo].[APIgetEmployeeWorkLocations]
	@employeeLocationID int
	
AS
BEGIN
	IF @employeeLocationID = 0
		BEGIN
			SELECT [employeeLocationID]
			  ,[employeeID]
			  ,[locationID]
			  ,[startDate]
			  ,[endDate]
			  ,[active]
			  ,[temporary]
			  ,[CreatedOn]
			  ,[CreatedBy]
			  ,[ModifiedOn]
			  ,[ModifiedBy]
		  FROM [dbo].[employeeWorkLocations]
		END
	ELSE
		BEGIN
			SELECT [employeeLocationID]
			  ,[employeeID]
			  ,[locationID]
			  ,[startDate]
			  ,[endDate]
			  ,[active]
			  ,[temporary]
			  ,[CreatedOn]
			  ,[CreatedBy]
			  ,[ModifiedOn]
			  ,[ModifiedBy]
		  FROM [dbo].[employeeWorkLocations]
		  WHERE [employeeLocationID] = @employeeLocationID
		END
END
RETURN 0
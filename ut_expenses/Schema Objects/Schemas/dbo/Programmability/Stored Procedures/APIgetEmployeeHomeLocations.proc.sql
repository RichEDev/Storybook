CREATE PROCEDURE [dbo].[APIgetEmployeeHomeLocations]
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
			  ,[CreatedOn]
			  ,[CreatedBy]
			  ,[ModifiedOn]
			  ,[ModifiedBy]
		  FROM [dbo].[employeeHomeLocations]
		END
	ELSE
		BEGIN
			SELECT [employeeLocationID]
			  ,[employeeID]
			  ,[locationID]
			  ,[startDate]
			  ,[endDate]
			  ,[CreatedOn]
			  ,[CreatedBy]
			  ,[ModifiedOn]
			  ,[ModifiedBy]
		  FROM [dbo].[employeeHomeLocations]
		  WHERE [employeeLocationID] = @employeeLocationID
		END
END
RETURN 0
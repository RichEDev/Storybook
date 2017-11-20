CREATE PROCEDURE [dbo].[APIgetCostCodes]
	@costcodeid int
AS
IF @costcodeid = 0
	BEGIN
		SELECT [costcodeid]
		  ,[costcode]
		  ,[description]
		  ,[archived]
		  ,[CreatedOn]
		  ,[CreatedBy]
		  ,[ModifiedOn]
		  ,[ModifiedBy]
	  FROM [dbo].[costcodes]
	END
ELSE
	BEGIN
		SELECT [costcodeid]
		  ,[costcode]
		  ,[description]
		  ,[archived]
		  ,[CreatedOn]
		  ,[CreatedBy]
		  ,[ModifiedOn]
		  ,[ModifiedBy]
	  FROM [dbo].[costcodes]
	  WHERE [costcodeid] = @costcodeid
	END
	
RETURN 0
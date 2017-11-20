CREATE PROCEDURE [dbo].[APIgetEsrElementFields]
	@elementFieldID int 
AS
BEGIN
IF  @elementFieldID = 0
	BEGIN
		SELECT [elementFieldID]
			  ,[elementID]
			  ,[globalElementFieldID]
			  ,[aggregate]
			  ,[order]
			  ,[reportColumnID]
		  FROM [dbo].[ESRElementFields]
	END
ELSE
	BEGIN
		SELECT [elementFieldID]
			  ,[elementID]
			  ,[globalElementFieldID]
			  ,[aggregate]
			  ,[order]
			  ,[reportColumnID]
		  FROM [dbo].[ESRElementFields]
		  WHERE elementFieldID = @elementFieldID
	END
END
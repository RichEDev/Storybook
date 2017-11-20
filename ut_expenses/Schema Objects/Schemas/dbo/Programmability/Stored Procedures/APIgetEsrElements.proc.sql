CREATE PROCEDURE [dbo].[APIgetEsrElements]
	@elementID int 
	
AS
BEGIN
IF @elementID = 0
	BEGIN
		SELECT [elementID]
		  ,[globalElementID]
		  ,[NHSTrustID]
	  FROM [dbo].[ESRElements]
	END
ELSE
	BEGIN
		SELECT [elementID]
		  ,[globalElementID]
		  ,[NHSTrustID]
	  FROM [dbo].[ESRElements]
	  WHERE elementID = @elementID
	END
END
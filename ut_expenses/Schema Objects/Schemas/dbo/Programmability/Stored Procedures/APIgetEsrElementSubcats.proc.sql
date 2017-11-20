CREATE PROCEDURE [dbo].[APIgetEsrElementSubCats]
	@elementSubcatID int 
	
AS
BEGIN
IF @elementSubcatID = 0
	BEGIN
		SELECT [elementSubcatID]
		  ,[elementID]
		  ,[subcatID]
	  FROM [dbo].[ESRElementSubcats]
	END
ELSE
	BEGIN
		SELECT [elementSubcatID]
		  ,[elementID]
		  ,[subcatID]
	  FROM [dbo].[ESRElementSubcats]
	  WHERE elementSubcatID = @elementSubcatID
	END
END
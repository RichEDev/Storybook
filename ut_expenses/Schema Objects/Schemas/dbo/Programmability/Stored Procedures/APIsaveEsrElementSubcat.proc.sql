

CREATE PROCEDURE [dbo].[APIsaveEsrElementSubcat]
	@elementSubcatID int out,
	@elementID int,
	@subcatID int
AS
BEGIN
IF @elementSubcatID = 0
	BEGIN
		INSERT INTO [dbo].[ESRElementSubcats]
			   ([elementID]
			   ,[subcatID])
		 VALUES
			   (@elementID,
			   @subcatID)
		set @elementSubcatID = scope_identity();
	END
ELSE
	BEGIN
		UPDATE [dbo].[ESRElementSubcats]
	   SET [elementID] = @elementID
		  ,[subcatID] = @subcatID
	 WHERE @elementSubcatID = elementSubcatID
	END
END
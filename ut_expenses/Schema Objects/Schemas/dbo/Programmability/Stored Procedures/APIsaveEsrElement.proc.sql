

CREATE PROCEDURE [dbo].[APIsaveEsrElement]
	@elementID int out,
	@globalElementID int,
    @NHSTrustID int
AS
BEGIN
	IF @elementID = 0
	BEGIN
		INSERT INTO [dbo].[ESRElements]
           ([globalElementID]
           ,[NHSTrustID])
		VALUES
           (@globalElementID
           ,@NHSTrustID)
		set @elementID = scope_identity();
	END
ELSE
	BEGIN
		UPDATE [dbo].[ESRElements]
	   SET [globalElementID] = @globalElementID,
		  [NHSTrustID] = @NHSTrustID
	 WHERE @elementID = elementID
	END
END
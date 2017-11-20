

CREATE PROCEDURE [dbo].[APIsaveEsrElementFields]
		@elementFieldID int out,
		@elementID int,
        @globalElementFieldID int,
        @aggregate tinyint,
        @order tinyint,
        @reportColumnID uniqueidentifier
AS
BEGIN
	IF @elementFieldID = 0
		BEGIN
			INSERT INTO [dbo].[ESRElementFields]
           ([elementID]
           ,[globalElementFieldID]
           ,[aggregate]
           ,[order]
           ,[reportColumnID])
     VALUES
           (@elementID,
           @globalElementFieldID,
           @aggregate, 
           @order, 
           @reportColumnID)
		   set @elementFieldID = scope_identity();
		END
	ELSE
		BEGIN
			UPDATE [dbo].[ESRElementFields]
		   SET [elementID] = @elementID
			  ,[globalElementFieldID] = @globalElementFieldID
			  ,[aggregate] = @aggregate
			  ,[order] = @order
			  ,[reportColumnID] = @reportColumnID
		 WHERE @elementFieldID = elementFieldID
		END
END
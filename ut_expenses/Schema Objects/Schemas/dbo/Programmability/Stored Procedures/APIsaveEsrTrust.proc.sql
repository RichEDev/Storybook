

CREATE PROCEDURE [dbo].[APIsaveEsrTrust]
	@trustID int out,
	@trustVPD nvarchar(3), 
    @periodType nvarchar(1), 
    @periodRun nvarchar(1), 
    @runSequenceNumber int, 
    @ftpAddress nvarchar(100), 
    @ftpUsername nvarchar(100), 
    @ftpPassword nvarchar(100), 
    @archived bit, 
    @createdOn datetime, 
    @modifiedOn datetime, 
    @trustName nvarchar(150), 
    @delimiterCharacter nvarchar(5),
	@ESRVersionNumber tinyint,
	@currentOutboundSequence int
AS
BEGIN
IF @trustID = 0
	BEGIN
		INSERT INTO [dbo].[esrTrusts]
			   ([trustVPD]
			   ,[periodType]
			   ,[periodRun]
			   ,[runSequenceNumber]
			   ,[ftpAddress]
			   ,[ftpUsername]
			   ,[ftpPassword]
			   ,[archived]
			   ,[createdOn]
			   ,[modifiedOn]
			   ,[trustName]
			   ,[delimiterCharacter]
			   ,[ESRVersionNumber]
			   ,[currentOutboundSequence])
		 VALUES
			   (@trustVPD , 
			   @periodType , 
			   @periodRun , 
			   @runSequenceNumber , 
			   @ftpAddress , 
			   @ftpUsername , 
			   @ftpPassword , 
			   @archived , 
			   @createdOn , 
			   @modifiedOn , 
			   @trustName , 
			   @delimiterCharacter,
			   @ESRVersionNumber,
			   @currentOutboundSequence )
			set @trustID = scope_identity();
	END
ELSE
	BEGIN
		UPDATE [dbo].[esrTrusts]
		   SET [trustVPD] = @trustVPD , 
			  [periodType] = @periodType , 
			  [periodRun] = @periodRun , 
			  [runSequenceNumber] = @runSequenceNumber , 
			  [ftpAddress] = @ftpAddress , 
			  [ftpUsername] = @ftpUsername , 
			  [ftpPassword] = @ftpPassword , 
			  [archived] = @archived , 
			  [createdOn] = @createdOn , 
			  [modifiedOn] = @modifiedOn , 
			  [trustName] = @trustName , 
			  [delimiterCharacter] = @delimiterCharacter ,
			  [ESRVersionNumber] = @ESRVersionNumber,
			  [currentOutboundSequence] = @currentOutboundSequence
		 WHERE trustID = @trustID
	END
END
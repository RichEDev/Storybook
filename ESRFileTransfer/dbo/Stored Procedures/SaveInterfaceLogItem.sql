CREATE PROCEDURE [dbo].[SaveInterfaceLogItem] 
      @LogItemID int,
      @LogItemType tinyint,
      @TransferType tinyint,
      @NHSTrustID int,
      @Filename nvarchar(250),
      @EmailSent bit,
      @LogItemBody nvarchar(max)
AS

      IF @LogItemID > 0
            BEGIN
                  UPDATE InterfaceLog SET EmailSent = @EmailSent, ModifiedOn = getdate() WHERE LogItemID = @LogItemID
            END
      ELSE
            BEGIN
                  INSERT INTO InterfaceLog (LogItemType, TransferType, NHSTrustID, [Filename], EmailSent, LogItemBody, CreatedOn) VALUES (@LogItemType, @TransferType, @NHSTrustID, @Filename, @EmailSent, @LogItemBody, getdate())
                  SET @LogItemID = scope_identity();
            END
            
      return @LogItemID

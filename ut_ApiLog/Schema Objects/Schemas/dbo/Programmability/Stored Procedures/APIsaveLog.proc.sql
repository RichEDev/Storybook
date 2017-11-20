CREATE PROCEDURE [dbo].[APIsaveLog]
    @LogItemType SMALLINT , 
	@AccountId INT,
	@NhsVpd nvarchar(3),
	@metabase nvarchar(100),
    @TransferType SMALLINT, 
    @LogReason SMALLINT , 
    @Filename NVARCHAR(100), 
    @LogId int , 
    @Message NVARCHAR(1000) , 
    @Source NVARCHAR(1000)  ,
	@messageLevel SMALLINT

AS
	insert into APILog ([AccountId],[NhsVpd],[metaBase], [TransferType], [LogItemType], [Filename], [LogId], [Message], [Source], [messageLevel], [CreatedOn], [LogItemreason]) 
				values (@AccountId, @NhsVpd, @metabase, @TransferType, @LogItemType, @Filename, @LogId, @Message, @Source, @messageLevel, GETUTCDATE(), @LogReason);
RETURN 0
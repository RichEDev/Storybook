CREATE PROCEDURE [dbo].[ApiBatchSaveEsrAddress]
    @list ApiBatchSaveEsrAddressType READONLY
AS 
BEGIN
    DECLARE @index bigint
    DECLARE @count bigint
    DECLARE @ESRAddressId bigint
    DECLARE @ESRPersonId bigint
    DECLARE @AddressType nvarchar(30)
    DECLARE @AddressStyle nvarchar(30)
    DECLARE @PrimaryFlag nvarchar(30)
    DECLARE @AddressLine1 nvarchar(240)
    DECLARE @AddressLine2 nvarchar(240)
    DECLARE @AddressLine3 nvarchar(240)
    DECLARE @AddressTown nvarchar(30)
    DECLARE @AddressCounty nvarchar(70)
    DECLARE @AddressPostcode nvarchar(30)
    DECLARE @AddressCountry nvarchar(60)
    DECLARE @EffectiveStartDate datetime
    DECLARE @EffectiveEndDate datetime
    DECLARE @ESRLastUpdate datetime

declare @tmp table (
    tmpID bigint,
    ESRAddressId bigint
)

insert @tmp
    select ROW_NUMBER() OVER (ORDER BY ESRAddressId), ESRAddressId from @list
 
select @count = count(*) from @tmp
set @index = 1


while @index <= @count
begin
    set @ESRAddressId = (select TOP 1 ESRAddressId from @tmp where tmpID = @index);

    SELECT @ESRPersonId = ESRPersonId,
		  @AddressType = AddressType,
		  @AddressStyle = AddressStyle,
		  @PrimaryFlag = PrimaryFlag,
		  @AddressLine1 = AddressLine1,
		  @AddressLine2 = AddressLine2,
		  @AddressLine3 = AddressLine3,
		  @AddressTown = AddressTown,
		  @AddressCounty = AddressCounty,
		  @AddressPostcode = AddressPostcode,
		  @AddressCountry = AddressCountry,
		  @EffectiveStartDate = EffectiveStartDate,
		  @EffectiveEndDate = EffectiveEndDate,
		  @ESRLastUpdate = ESRLastUpdate
    FROM @list WHERE ESRAddressId = @ESRAddressId
		      

	IF NOT EXISTS(SELECT * FROM ESRAddresses WHERE ESRAddressId = @ESRAddressID)
	BEGIN
	INSERT INTO [dbo].[ESRAddresses]
           ([ESRAddressId]
           ,[ESRPersonId]
           ,[AddressType]
           ,[AddressStyle]
           ,[PrimaryFlag]
           ,[AddressLine1]
           ,[AddressLine2]
           ,[AddressLine3]
           ,[AddressTown]
           ,[AddressCounty]
           ,[AddressPostcode]
           ,[AddressCountry]
           ,[EffectiveStartDate]
           ,[EffectiveEndDate]
           ,[ESRLastUpdate])
     VALUES
           (@ESRAddressId
           ,@ESRPersonId
           ,@AddressType
           ,@AddressStyle
           ,@PrimaryFlag
           ,@AddressLine1
           ,@AddressLine2
           ,@AddressLine3
           ,@AddressTown
           ,@AddressCounty
           ,@AddressPostcode
           ,@AddressCountry
           ,@EffectiveStartDate
           ,@EffectiveEndDate
           ,@ESRLastUpdate)
	END
ELSE
	BEGIN
	UPDATE [dbo].[ESRAddresses]
   SET [ESRAddressId] = @ESRAddressId
      ,[ESRPersonId] = @ESRPersonId
      ,[AddressType] = @AddressType
      ,[AddressStyle] = @AddressStyle
      ,[PrimaryFlag] = @PrimaryFlag
      ,[AddressLine1] = @AddressLine1
      ,[AddressLine2] = @AddressLine2
      ,[AddressLine3] = @AddressLine3
      ,[AddressTown] = @AddressTown
      ,[AddressCounty] = @AddressCounty
      ,[AddressPostcode] = @AddressPostcode
      ,[AddressCountry] = @AddressCountry
      ,[EffectiveStartDate] = @EffectiveStartDate
      ,[EffectiveEndDate] = @EffectiveEndDate
      ,[ESRLastUpdate] = @ESRLastUpdate
	WHERE ESRAddressId = @ESRAddressID
	END


  set @index= @index + 1
end

    return 0;
end
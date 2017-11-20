

CREATE PROCEDURE [dbo].[APIsaveEsrAddress]
			@ESRAddressId bigint out
           ,@ESRPersonId bigint
           ,@AddressType nvarchar(30)
           ,@AddressStyle nvarchar(30)
           ,@PrimaryFlag nvarchar(30)
           ,@AddressLine1 nvarchar(240)
           ,@AddressLine2 nvarchar(240)
           ,@AddressLine3 nvarchar(240)
           ,@AddressTown nvarchar(30)
           ,@AddressCounty nvarchar(70)
           ,@AddressPostcode nvarchar(30)
           ,@AddressCountry nvarchar(60)
           ,@EffectiveStartDate datetime
           ,@EffectiveEndDate datetime
           ,@ESRLastUpdate datetime
AS
BEGIN
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
END
CREATE PROCEDURE [dbo].[ApiBatchSaveEsrLocation] @list ApiBatchSaveEsrLocationType READONLY
AS
BEGIN
 DECLARE @index BIGINT
 DECLARE @count BIGINT
 DECLARE @ESRLocationId [bigint]
 DECLARE @LocationCode [nvarchar] (60)
 DECLARE @Description [nvarchar] (240)
 DECLARE @InactiveDate [datetime] = NULL
 DECLARE @AddressLine1 [nvarchar] (240)
 DECLARE @AddressLine2 [nvarchar] (240)
 DECLARE @AddressLine3 [nvarchar] (240)
 DECLARE @Town [nvarchar] (30)
 DECLARE @County [nvarchar] (70)
 DECLARE @Postcode [nvarchar] (30)
 DECLARE @Country [nvarchar] (60)
 DECLARE @Telephone [nvarchar] (60)
 DECLARE @Fax [nvarchar] (60)
 DECLARE @PayslipDeliveryPoint [nvarchar] (1)
 DECLARE @SiteCode [nvarchar] (2)
 DECLARE @WelshLocationTranslation [nvarchar] (60)
 DECLARE @WelshAddress1 [nvarchar] (60)
 DECLARE @WelshAddress2 [nvarchar] (60)
 DECLARE @WelshAddress3 [nvarchar] (60)
 DECLARE @WelshTownTranslation [nvarchar] (60)
 DECLARE @ESRLastUpdate [datetime] = NULL
 DECLARE @tmp TABLE (
  tmpID BIGINT
  ,EsrLocationId BIGINT
  )

 INSERT @tmp
 SELECT ROW_NUMBER() OVER (
   ORDER BY EsrLocationId
   )
  ,EsrLocationId
 FROM @list

 SELECT @count = count(*)
 FROM @tmp

 SET @index = 1

 WHILE @index <= @count
 BEGIN
  SET @ESRLocationId = (
    SELECT TOP 1 ESRLocationId
    FROM @tmp
    WHERE tmpID = @index
    );

  SELECT TOP 1 @LocationCode = [LocationCode]
   ,@Description = [Description]
   ,@InactiveDate = [InactiveDate]
   ,@AddressLine1 = [AddressLine1]
   ,@AddressLine2 = [AddressLine2]
   ,@AddressLine3 = [AddressLine3]
   ,@Town = [Town]
   ,@County = [County]
   ,@Postcode = [Postcode]
   ,@Country = [Country]
   ,@Telephone = [Telephone]
   ,@Fax = [Fax]
   ,@PayslipDeliveryPoint = [PayslipDeliveryPoint]
   ,@SiteCode = [SiteCode]
   ,@WelshLocationTranslation = [WelshLocationTranslation]
   ,@WelshAddress1 = [WelshAddress1]
   ,@WelshAddress2 = [WelshAddress2]
   ,@WelshAddress3 = [WelshAddress3]
   ,@WelshTownTranslation = [WelshTownTranslation]
   ,@ESRLastUpdate = [ESRLastUpdate]
  FROM @list
  WHERE ESRLocationId = @ESRLocationId

  IF NOT EXISTS (
    SELECT ESRLocationId
    FROM ESRLocations
    WHERE ESRLocationId = @ESRLocationId
    )
  BEGIN
   INSERT INTO [dbo].[ESRLocations] (
    [ESRLocationId]
    ,[LocationCode]
    ,[Description]
    ,[InactiveDate]
    ,[AddressLine1]
    ,[AddressLine2]
    ,[AddressLine3]
    ,[Town]
    ,[County]
    ,[Postcode]
    ,[Country]
    ,[Telephone]
    ,[Fax]
    ,[PayslipDeliveryPoint]
    ,[SiteCode]
    ,[WelshLocationTranslation]
    ,[WelshAddress1]
    ,[WelshAddress2]
    ,[WelshAddress3]
    ,[WelshTownTranslation]
    ,[ESRLastUpdate]
    )
   VALUES (
    @ESRLocationId
    ,@LocationCode
    ,@Description
    ,@InactiveDate
    ,@AddressLine1
    ,@AddressLine2
    ,@AddressLine3
    ,@Town
    ,@County
    ,@Postcode
    ,@Country
    ,@Telephone
    ,@Fax
    ,@PayslipDeliveryPoint
    ,@SiteCode
    ,@WelshLocationTranslation
    ,@WelshAddress1
    ,@WelshAddress2
    ,@WelshAddress3
    ,@WelshTownTranslation
    ,@ESRLastUpdate
    )
  END
  ELSE
  BEGIN
   UPDATE [dbo].[ESRLocations]
   SET [ESRLocationId] = @ESRLocationId
    ,[LocationCode] = @LocationCode
    ,[Description] = @Description
    ,[InactiveDate] = @InactiveDate
    ,[AddressLine1] = @AddressLine1
    ,[AddressLine2] = @AddressLine2
    ,[AddressLine3] = @AddressLine3
    ,[Town] = @Town
    ,[County] = @County
    ,[Postcode] = @Postcode
    ,[Country] = @Country
    ,[Telephone] = @Telephone
    ,[Fax] = @Fax
    ,[PayslipDeliveryPoint] = @PayslipDeliveryPoint
    ,[SiteCode] = @SiteCode
    ,[WelshLocationTranslation] = @WelshLocationTranslation
    ,[WelshAddress1] = @WelshAddress1
    ,[WelshAddress2] = @WelshAddress2
    ,[WelshAddress3] = @WelshAddress3
    ,[WelshTownTranslation] = @WelshTownTranslation
    ,[ESRLastUpdate] = @ESRLastUpdate
   WHERE ESRLocationId = @ESRLocationId
   
   -- if the location has changed and it is linked to an address then update the address values too
   DECLARE @AddressId INT = NULL;
   SELECT @AddressId = AddressId FROM AddressEsrAllocation WHERE ESRLocationId = @ESRLocationId;
   
   IF (@AddressId IS NOT NULL)
   BEGIN
		-- if the postcode has changed then drop any related distance data
		DECLARE @CurrentPostcode NVARCHAR(30);
		SELECT @CurrentPostcode = Postcode FROM Addresses WHERE AddressId = @AddressId;

		IF (NOT EXISTS (SELECT @CurrentPostcode INTERSECT SELECT @PostCode))
		BEGIN
			DELETE FROM addressDistances WHERE AddressIdA = @AddressId OR AddressIdB = @AddressId;
		END
   
		UPDATE [dbo].[addresses]
		SET 
		[AddressName] = @Description
		,[Line1] = @AddressLine1
		,[Line2] = @AddressLine2
		,[Line3] = @AddressLine3
		,[City] = @Town
		,[County] = @County
		,[Postcode] = @Postcode
		,[AddressNameLookup] = dbo.CleanLookup(@Description)
		,[Line1Lookup] = dbo.CleanLookup(@AddressLine1)
		,[Line2Lookup] = dbo.CleanLookup(@AddressLine2)
		,[CityLookup] = dbo.CleanLookup(@Town)
		,[PostcodeLookup] = dbo.CleanLookup(@Postcode)
		WHERE [AddressID] = @AddressID;
   END
  END

  SET @index = @index + 1
 END

 RETURN 0;
END
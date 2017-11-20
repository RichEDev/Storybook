CREATE PROCEDURE [dbo].[APIgetEsrPhones]
	@ESRPhoneId bigint 
AS
BEGIN
	IF @ESRPhoneId = 0
	BEGIN
		SELECT [ESRPhoneId]
		  ,[ESRPersonId]
		  ,[PhoneType]
		  ,[PhoneNumber]
		  ,[EffectiveStartDate]
		  ,[EffectiveEndDate]
		  ,[ESRLastUpdate]
	  FROM [dbo].[ESRPhones]
	END
	ELSE
	BEGIN
		SELECT [ESRPhoneId]
		  ,[ESRPersonId]
		  ,[PhoneType]
		  ,[PhoneNumber]
		  ,[EffectiveStartDate]
		  ,[EffectiveEndDate]
		  ,[ESRLastUpdate]
	  FROM [dbo].[ESRPhones]
	  WHERE [ESRPhoneId] = @ESRPhoneId;
	END
END
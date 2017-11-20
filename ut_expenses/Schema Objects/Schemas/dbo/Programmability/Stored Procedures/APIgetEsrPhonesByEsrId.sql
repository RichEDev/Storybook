
CREATE PROCEDURE [dbo].[APIgetEsrPhonesByEsrId]
 @EsrId bigint 
AS
BEGIN
 SELECT [ESRPhoneId]
  ,[ESRPersonId]
  ,[PhoneType]
  ,[PhoneNumber]
  ,[EffectiveStartDate]
  ,[EffectiveEndDate]
  ,[ESRLastUpdate]
 FROM [dbo].[ESRPhones]
 WHERE [ESRPersonId] = @EsrId;
END
CREATE PROCEDURE [dbo].[APIdeleteEsrPhone]
	@ESRPhoneId bigint 
AS
DELETE FROM [dbo].[ESRPhones]
      WHERE [dbo].[ESRPhones].[ESRPhoneId] = @ESRPhoneId;
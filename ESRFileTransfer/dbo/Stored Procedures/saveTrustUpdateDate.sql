CREATE PROCEDURE [dbo].[saveTrustUpdateDate] 
      
@Date datetime
AS
BEGIN
      UPDATE globalProperties SET LastTrustUpdateDateCheck = @Date;
END

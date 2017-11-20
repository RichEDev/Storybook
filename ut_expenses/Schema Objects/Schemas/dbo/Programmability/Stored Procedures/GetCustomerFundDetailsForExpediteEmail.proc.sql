CREATE PROC [dbo].[GetCustomerFundDetailsForExpediteEmail]
@accountId int

AS

BEGIN

DECLARE @dbName varchar(50)
DECLARE @sqlScript varchar(MAX)
DECLARE @template VARCHAR(MAX)
DECLARE @fundLimit decimal(18,2)
DECLARE @fundInVarchar VARCHAR(MAX)
DECLARE @accountIdInVarchar varchar(10)

SELECT @dbName = dbname, @fundLimit= FundLimit  FROM registeredusers WHERE accountid = @accountId
SELECT @fundInVarchar = CONVERT(varchar(Max), @fundLimit)
SELECT @accountIdInVarchar = CONVERT(varchar(10),@accountId)
SET @template = 'DECLARE @AvailableFund decimal(18,2) DECLARE @AdminId int DECLARE @EmailServerAddress varchar(100) SELECT @EmailServerAddress = stringValue from {DATABASE}.dbo.accountProperties where stringKey =''emailServerAddress''   SELECT @AdminId = stringValue FROM  {DATABASE}.dbo.accountProperties WHERE stringKey=''mainAdministrator'' SELECT @AvailableFund= AvailableFund FROM {DATABASE}.dbo.FundTransaction SELECT '+ @fundInVarchar + 'as FundLimit,' + @fundInVarchar + '- @AvailableFund as MinTopUpRequired , email as Email,firstname as FirstName,surname as SurName, @EmailServerAddress as EmailServerAddress,'+ @accountIdInVarchar  + 'as accountId FROM {DATABASE}.dbo.employees WHERE employeeid = @AdminId ' 
SET @sqlScript = REPLACE(@template, '{DATABASE}', @dbName)

EXECUTE (@sqlScript)

END
GO
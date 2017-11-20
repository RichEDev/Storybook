CREATE PROCEDURE dbo.GetDefaultCostCodeOwner(@subAccountId int, @ownerType int output, @ownerId int output)
AS
BEGIN
	SET @ownerId = 0;
	SET @ownerType = -1;
	
	declare @defaultCCO_Key nvarchar(20)
	select @defaultCCO_Key = stringValue from accountProperties where stringKey = 'defaultCostCodeOwner' and subAccountID = @subAccountId

	if (select COUNT(Value) from dbo.UTILfn_Split(@defaultCCO_Key, ',')) = 2
	BEGIN	
		declare @separatorIdx int = (select CHARINDEX(',', @defaultCCO_Key));
		
		SET @ownerId = CAST((SELECT SUBSTRING(@defaultCCO_Key, @separatorIdx+1, LEN(@defaultCCO_Key)+1)) AS INT)
		SET @ownerType = CAST((SELECT SUBSTRING(@defaultCCO_Key, 1, @separatorIdx-1)) AS INT)
	END
END
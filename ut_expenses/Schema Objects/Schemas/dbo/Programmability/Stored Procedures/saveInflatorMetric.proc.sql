CREATE PROCEDURE [dbo].[saveInflatorMetric] 
(
@ID int, 
@subAccountId int, 
@name nvarchar(50),
@percentage decimal(18,2),
@requiresExtraPct bit,
@employeeId int,
@delegateID int
)
AS
DECLARE @count INT;
DECLARE @retVal INT;
DECLARE @recordTitle nvarchar(2000);

IF @ID = -1
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_inflatormetrics WHERE name = @name AND subAccountId = @subAccountId);
	
	IF @count = 0
	BEGIN
		INSERT INTO codes_inflatormetrics (subAccountId, name, percentage, requiresExtraPct, archived, createdOn, createdBy)
		VALUES (@subAccountId, @name, @percentage, @requiresExtraPct, 0, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Inflator Metric ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @name + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 112, @retVal, @recordTitle, @subAccountId;
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_inflatormetrics WHERE metricId <> @ID AND subAccountId = @subAccountId AND name = @name);
	
	IF @count = 0
	BEGIN
		DECLARE @oldname nvarchar(50);
		DECLARE @oldrequireExtraPct bit;
		DECLARE @oldPercentage decimal(18,2);
		
		select @oldname = name, @oldPercentage=percentage, @oldrequireExtraPct=requiresExtraPct from codes_inflatormetrics where metricId = @ID;
		
		UPDATE codes_inflatormetrics SET name = @name, percentage = @percentage, requiresExtraPct = @requiresExtraPct, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE metricId = @ID;
		
		SET @retVal = @ID;
		
		set @recordTitle = 'Inflator Metric ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @name + ')';

		if @oldname <> @name
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 112, @ID, 'BCDF9634-741D-4F07-80E7-4B3F6CAA2CC7', @oldname, @name, @recordtitle, @subAccountId;
			
		if @oldPercentage <> @percentage
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 112, @ID, '6670A854-4F18-4966-8540-43CFBE0BF4E0', @oldPercentage, @percentage, @recordtitle, @subAccountId;

		if @oldrequireExtraPct <> @requiresExtraPct
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 112, @ID, '196209D1-DC11-4CF2-B1AE-276F4EC60FFD', @oldrequireExtraPct, @requiresExtraPct, @recordtitle, @subAccountId;

	END
END

RETURN @retVal; 

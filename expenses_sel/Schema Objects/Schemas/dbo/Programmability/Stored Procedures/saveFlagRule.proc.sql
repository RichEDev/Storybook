
CREATE PROCEDURE [dbo].[saveFlagRule]
	@flagID int,
	@flagType TINYINT,
	@action TINYINT,
	@flagText NVARCHAR(MAX),
	@amberTolerance TINYINT,
	@redTolerance TINYINT, 
	@frequency TINYINT,
	@period TINYINT,
	@periodType TINYINT,
	@limit DECIMAL(18,2),
	@dateComparison TINYINT,
	@dateToCompare DATETIME,
	@numberOfMonths TINYINT,
	@description NVARCHAR(MAX),
	@active BIT,
	@date DATETIME,
	@userid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    IF @flagID > 0
		BEGIN
			UPDATE flags SET [action] = @action, flagText = @flagText, amberTolerance = @amberTolerance, redTolerance = @redtolerance, frequency = @frequency, period = @period, periodType = @periodType, limit = @limit, dateComparisonType = @dateComparison, dateToCompare = @dateToCompare, numberOfMonths = @numberofmonths, modifiedOn = @date, modifiedBy = @userid, [description] = @description, active = @active WHERE flagID = @flagID
		end
	ELSE
		BEGIN
			INSERT INTO flags (flagtype, [action], flagtext, amberTolerance, redTolerance, frequency,period, periodType, limit, dateComparisonType, dateToCompare, numberOfMonths, createdOn, createdBy, [description], active) VALUES (@flagType, @action, @flagText, @amberTolerance, @redTolerance, @frequency, @period, @periodType, @limit, @dateComparison, @dateToCompare, @numberOfMonths, @date, @userid, @description, @active)
			SET @flagID = SCOPE_IDENTITY()
		END
		
	RETURN @flagID
END

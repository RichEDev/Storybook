CREATE PROCEDURE [dbo].[PopulateStaticRanges]
	@date DateTime
AS
	DECLARE @employeeid INT;
IF isnull(@DATE, '1900-01-01') = '1900-01-01'
BEGIN
	SELECT @date = min([date]) FROM savedexpenses WHERE date IS NOT NULL
END



DECLARE @currencyId int;
DECLARE @toCurrencyId int;
DECLARE @exchangerate float;
DECLARE @currencyRangeId int;
DECLARE @currentCurrencyId int;

DECLARE staticCursor CURSOR
FOR SELECT currencyid, tocurrencyid, exchangerate FROM static_exchangerates
OPEN staticCursor
FETCH NEXT FROM staticCursor INTO @currencyId, @toCurrencyId, @exchangerate

WHILE @@FETCH_STATUS = 0 
BEGIN
	IF @date < GETDATE()
	BEGIN
		IF ISNULL(@currentCurrencyId,0) <> @currencyId
		BEGIN
			INSERT INTO currencyranges (currencyid, startdate, enddate, createdon, createdby) VALUES(@currencyId, @date, CAST(GETDATE() as Date),CAST(GETDATE() as Date),@employeeid)
			SET @currencyRangeId = @@IDENTITY
		END

		INSERT INTO range_exchangerates (currencyrangeid, tocurrencyid, exchangerate) VALUES (@currencyRangeId, @toCurrencyId, @exchangerate)
		SET @currentCurrencyId = @currencyId
		FETCH NEXT FROM staticCursor INTO @currencyId, @toCurrencyId, @exchangerate
	END
END

CLOSE staticCursor   
DEALLOCATE staticCursor 


RETURN 0

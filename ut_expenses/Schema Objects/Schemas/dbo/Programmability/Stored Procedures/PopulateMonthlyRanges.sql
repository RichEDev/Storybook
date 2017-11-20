CREATE PROCEDURE [dbo].[PopulateMonthlyRanges]
	@date DateTime
AS
	DECLARE @EMPLOYEEID INT;
IF isnull(@DATE, '1900-01-01') = '1900-01-01'
BEGIN
	SELECT @date = min([date]) FROM savedexpenses WHERE date IS NOT NULL
END



DECLARE @currencyId int;
DECLARE @startDate DateTime;
DECLARE @endDate DateTime;
DECLARE @toCurrencyId int;
DECLARE @exchangerate float;
DECLARE @currencyRangeId int;
DECLARE @currentCurrencyId int;
DECLARE @currentStartDate Date;

DECLARE monthlyCursor CURSOR
FOR select currencyid, CAST(CAST([YEAR] AS varchar) + '-' + CAST([MONTH] AS varchar) + '-' + CAST(1 AS varchar) AS DATE) as startDate,
CAST(DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,CAST(CAST([YEAR] AS varchar) + '-' + CAST([MONTH] AS varchar) + '-' + CAST(1 AS varchar) AS DATETIME))+1,0)) as Date) as endDate,  tocurrencyid, monthly_exchangerates.exchangerate from currencymonths inner join monthly_exchangerates on currencymonths.currencymonthid = monthly_exchangerates.currencymonthid
OPEN monthlyCursor
FETCH NEXT FROM monthlyCursor INTO @currencyId, @startDate, @endDate, @toCurrencyId, @exchangerate

WHILE @@FETCH_STATUS = 0 
BEGIN
	IF @startDate >= @date AND @startDate < GETDATE()
	BEGIN
		IF @endDate > GETDATE()
		BEGIN
			SET @endDate = GETDATE();
		END
		IF ISNULL(@currentCurrencyId,0) <> @currencyId OR ISNULL(@currentStartDate, '1900-01-01') <> @startDate
		BEGIN
			INSERT INTO currencyranges (currencyid, startdate, enddate, createdon, createdby) VALUES(@currencyId, @startDate,@endDate, GETDATE(), @employeeid)
			SET @currencyRangeId = @@IDENTITY
		END
		SET @currentCurrencyId = @currencyId
		SET @currentStartDate = @startDate
		INSERT INTO range_exchangerates (currencyrangeid, tocurrencyid, exchangerate) VALUES (@currencyRangeId, @toCurrencyId, @exchangerate)
	END
	FETCH NEXT FROM monthlyCursor INTO @currencyId, @startDate, @endDate, @toCurrencyId, @exchangerate
END

CLOSE monthlyCursor   
DEALLOCATE monthlyCursor 

RETURN 0

CREATE PROCEDURE [dbo].[GenerateVehicleJourneyRateStandardReport]
AS

declare @cols nvarchar(max) = '',
		@cols_ppm nvarchar(max) = '',
		@cols_ppm_names nvarchar(max) = '',
		@cols_vat nvarchar(max) = '',
		@cols_vat_names nvarchar(max) = '';

SELECT
	@cols = @cols + '[' + CAST(VehicleEngineTypeId as varchar) + '], ',
	@cols_ppm = @cols_ppm + '[' + CAST(VehicleEngineTypeId as varchar) + '] AS [£ per unit - ' + Name + '], ',
	@cols_ppm_names = @cols_ppm_names + '[sql_ppm].[£ per unit - ' + Name + '], ',
	@cols_vat = @cols_vat + '[' + CAST(VehicleEngineTypeId as varchar) + '] AS [£ per unit for VAT Reclaim - ' + Name + '], ',
	@cols_vat_names = @cols_vat_names + '[sql_vat].[£ per unit for VAT Reclaim - ' + Name + '], '
FROM
	VehicleEngineTypes
ORDER BY
	Name;

set @cols = SUBSTRING(@cols, 1, LEN(@cols) - 1);
set @cols_ppm = SUBSTRING(@cols_ppm, 1, LEN(@cols_ppm) - 1);
set @cols_ppm_names = SUBSTRING(@cols_ppm_names, 1, LEN(@cols_ppm_names) - 1);
set @cols_vat = SUBSTRING(@cols_vat, 1, LEN(@cols_vat) - 1);
set @cols_vat_names = SUBSTRING(@cols_vat_names, 1, LEN(@cols_vat_names) - 1);

declare @sql_ppm nvarchar(max) = '
SELECT
	MileageThresholdId,
    ' + @cols_ppm + '
FROM
(
	SELECT
		MileageThresholdId,
		VehicleEngineTypeId,
		RatePerUnit
	FROM
		MileageThresholdRates
) AS src
PIVOT
(
    MAX(RatePerUnit)
FOR
	VehicleEngineTypeId
    IN (' + @cols + ')
) AS pvtRatePerUnit';

declare @sql_vat nvarchar(max) = '
SELECT
	MileageThresholdId,
    ' + @cols_vat + '
FROM
(
	SELECT
		MileageThresholdId,
		VehicleEngineTypeId,
		AmountForVat
	FROM
		MileageThresholdRates
) AS src
PIVOT
(
    MAX(AmountForVat)
FOR
	VehicleEngineTypeId
    IN (' + @cols + ')
) AS pvtAmountForVat';

declare @sql nvarchar(max) = 'SELECT
	[carsize] AS [Vehicle Journey Rate],
	CASE [catvalid]
		WHEN ''1''
			THEN ''Yes''
		ELSE ''No''
		END AS [Is Journey Rate Valid?],
	[Rate invalid comment] = CASE [catvalid]
		WHEN ''1''
			THEN ''N/A''
		ELSE [catvalidcomment]
		END,
	CASE [thresholdtype]
		WHEN ''1''
			THEN ''Day''
		ELSE ''Annual''
		END AS [Threshold Type],
	[comment] AS [Description],
	[Label] AS ''Currency'',
	CASE [unit]
		WHEN ''0''
			THEN ''Miles''
		ELSE ''Kilometers''
		END AS [Unit of Measure],
	CASE [calcmilestotal]
		WHEN ''1''
			THEN ''Yes''
		ELSE ''No''
		END AS [Calculate New Journey Total],
	[Financialyears].[description] AS [Financial Year],
	CASE [daterangetype]
		WHEN ''0''
			THEN ''Before''
		WHEN ''1''
			THEN ''After or Equal to''
		WHEN ''2''
			THEN ''Between (inclusive)''
		WHEN ''3''
			THEN ''Any''
		ELSE ''No Date Range Defined''
		END AS [Date Range Type],
	[Date Value 1] = CASE [daterangetype]
		WHEN ''3''
			THEN NULL
		ELSE [datevalue1]
		END,
	[Date Value 2] = CASE [daterangetype]
		WHEN ''2''
			THEN [datevalue2]
		ELSE NULL
		END,
	CASE [Rangetype]
		WHEN ''0''
			THEN ''Greater than or Equal''
		WHEN ''1''
			THEN ''Between (Inclusive)''
		WHEN ''2''
			THEN ''Less Than''
		WHEN ''3''
			THEN ''Any''
		ELSE ''No Thresholds Defined''
		END AS [Threshold Range Type],
	[Range Value 1] = CASE [rangetype]
		WHEN ''3''
			THEN NULL
		ELSE [rangevalue1]
		END,
	[Range Value 2] = CASE [rangetype]
		WHEN ''1''
			THEN [Rangevalue2]
		ELSE NULL
		END,
	' + @cols_ppm_names + ',
	' + @cols_vat_names + ',
	[passenger1] AS [£ per unit - Passenger allowance - 1st passenger],
	[passengerx] AS [£ per unit - Passenger allowance - additional passengers],
	[Heavybulkyequipment] AS [£ per unit - Heavy/Bulky Goods Allowance]
FROM
	mileage_categories
		LEFT JOIN currencies ON mileage_categories.currencyid = currencies.currencyid
			LEFT JOIN FinancialYears ON mileage_categories.FinancialYearID = FinancialYears.FinancialYearID
				LEFT JOIN global_currencies ON currencies.globalcurrencyid = global_currencies.globalcurrencyid
					LEFT JOIN mileage_dateranges ON mileage_categories.mileageid = mileage_dateranges.mileageid
						LEFT JOIN mileage_thresholds ON mileage_dateranges.mileagedateid = mileage_thresholds.mileagedateid
							join (' + @sql_ppm + ') as sql_ppm on sql_ppm.MileageThresholdId = mileage_thresholds.mileagethresholdid
							join (' + @sql_vat + ') as sql_vat on sql_vat.MileageThresholdId = mileage_thresholds.mileagethresholdid
ORDER BY
	[carsize],
	[datevalue1],
	[datevalue2],
	[rangevalue1]';

print @sql;

exec sp_executesql @sql;

RETURN 0;

CREATE VIEW MileageCategoriesView
AS
SELECT DISTINCT MC.mileageid AS mileageid
	,carsize
	,unit
	,catvalid
	,financialYearID
	,VehicleEngineTypeId
FROM Mileage_categories MC
INNER JOIN mileage_dateranges MD ON MD.mileageid = MC.mileageid
INNER JOIN mileage_thresholds MT ON MT.mileagedateid = MD.mileagedateid
INNER JOIN MileageThresholdRates MTR ON MT.mileagethresholdid = MTR.MileageThresholdId


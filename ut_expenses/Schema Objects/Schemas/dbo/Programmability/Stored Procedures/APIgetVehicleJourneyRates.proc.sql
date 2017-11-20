CREATE PROCEDURE [dbo].[APIgetVehicleJourneyRates]
	@mileageid int
AS
	IF @mileageid = 0
		BEGIN
			SELECT mileageid, carsize, comment, calcmilestotal, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, thresholdtype, catvalid, unit, catvalidcomment, currencyid, UserRatesTable, UserRatesFromEngineSize, UserRatesToEngineSize 
				FROM mileage_categories
		END
	ELSE
		BEGIN
		SELECT mileageid, carsize, comment, calcmilestotal, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, thresholdtype, catvalid, unit, catvalidcomment, currencyid, UserRatesTable, UserRatesFromEngineSize, UserRatesToEngineSize 
				FROM mileage_categories
				WHERE mileageid = @mileageid
		END
RETURN 0

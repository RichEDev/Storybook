CREATE PROCEDURE [dbo].[GetMileageCategories]
	@mileageid int = null
AS
	SELECT
		mileageid, carsize, comment, thresholdtype, calcmilestotal, unit, catvalid,
		catvalidcomment, currencyid, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy,
		UserRatesTable, UserRatesFromEngineSize, UserRatesToEngineSize, FinancialYearID
	FROM
		dbo.mileage_categories
	WHERE
		mileageid = @mileageid
		or @mileageid is null
	ORDER BY
		carsize

RETURN 0

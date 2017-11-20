CREATE PROCEDURE [dbo].[GetMileageDateRanges]
	@mileagedateid int = null
AS
	SELECT
		[mileagedateid],
		[mileageid],
		[startdate],
		[enddate],
		[CreatedOn],
		[CreatedBy],
		[ModifiedOn],
		[ModifiedBy],
		[datevalue1],
		[datevalue2],
		[daterangetype]
	FROM
		[dbo].[mileage_dateranges]
	WHERE
		mileagedateid = @mileagedateid
		or @mileagedateid is null
		or @mileagedateid = 0
	ORDER BY
		[mileageid],
		[datevalue1],
		[daterangetype]

RETURN 0

CREATE PROCEDURE [dbo].[GetMileageThresholds]
	@mileagethresholdid int = null
AS
	SELECT
		[mileagethresholdid],
		[mileagedateid],
		[rangetype],
		[rangevalue1],
		[rangevalue2],
		[ppmpetrol],
		[ppmdiesel],
		[ppmlpg],
		[amountforvatp],
		[amountforvatd],
		[amountforvatlpg],
		[passenger1],
		[passengerx],
		[CreatedOn],
		[CreatedBy],
		[ModifiedOn],
		[ModifiedBy],
		[heavyBulkyEquipment],
		[ppmhybrid],
		[ppmelectric],
		[ppmdieseleurov],
		[amountforvathybrid],
		[amountforvatelectric],
		[amountforvatdieseleurov]
	FROM
		[dbo].[mileage_thresholds]
	WHERE
		[mileagethresholdid] = @mileagethresholdid
		or @mileagethresholdid is null
		or @mileagethresholdid = 0
	ORDER BY
		[mileagedateid],
		[rangevalue1],
		[rangetype] desc

RETURN 0

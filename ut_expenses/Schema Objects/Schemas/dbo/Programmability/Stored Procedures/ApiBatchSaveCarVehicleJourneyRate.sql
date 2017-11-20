
CREATE PROCEDURE [dbo].[ApiBatchSaveCarVehicleJourneyRate] @list ApiBatchSaveCarVehicleJourneyRateType READONLY
AS
BEGIN
	DECLARE @index BIGINT
	DECLARE @count BIGINT
	DECLARE @carid
 INT
	DECLARE @mileageid INT
	DECLARE @tmp TABLE (
		tmpID BIGINT
		,carid BIGINT
		,mileageid BIGINT
		)

	INSERT @tmp
	SELECT ROW_NUMBER() OVER (
			ORDER BY carid
				,mileageid
			)
		,carid
		,mileageid
	FROM @list

	SELECT @count = count(*)
	FROM @tmp

	SET @index = 1

	WHILE @index <= @count
	BEGIN
		SELECT top 1 @carid = carid
			,@mileageid = mileageid
		FROM @tmp
		WHERE tmpID = @index;

		IF NOT EXISTS (
				SELECT carid
				FROM car_mileagecats
				WHERE carid = @carid
					AND mileageid = @mileageid
				)
		BEGIN
			INSERT INTO car_mileagecats (
				carid
				,mileageid
				)
			VALUES (
				@carid
				,@mileageid
				);
		END

		SET @index = @index + 1
	END

	RETURN 0;
END
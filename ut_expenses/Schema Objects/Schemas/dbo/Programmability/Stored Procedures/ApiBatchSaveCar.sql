
CREATE PROCEDURE [dbo].[ApiBatchSaveCar] @list ApiBatchSaveCarType READONLY
AS
BEGIN
	DECLARE @index INT
	DECLARE @count INT
	DECLARE @carid INT
	DECLARE @employeeid INT
	DECLARE @startdate DATETIME
	DECLARE @enddate DATETIME
	DECLARE @make NVARCHAR(50)
	DECLARE @model NVARCHAR(50)
	DECLARE @registration NVARCHAR(50)
	DECLARE @mileageid INT
	DECLARE @cartypeid TINYINT
	DECLARE @active BIT
	DECLARE @odometer BIGINT
	DECLARE @fuelcard BIT
	DECLARE @endodometer INT
	DECLARE @taxexpiry DATETIME
	DECLARE @taxlastchecked DATETIME
	DECLARE @taxcheckedby INT
	DECLARE @mottestnumber NVARCHAR(50)
	DECLARE @motlastchecked DATETIME
	DECLARE @motcheckedby INT
	DECLARE @motexpiry DATETIME
	DECLARE @insurancenumber NVARCHAR(50)
	DECLARE @insuranceexpiry DATETIME
	DECLARE @insurancelastchecked DATETIME
	DECLARE @insurancecheckedby INT
	DECLARE @serviceexpiry DATETIME
	DECLARE @servicelastchecked DATETIME
	DECLARE @servicecheckedby INT
	DECLARE @CreatedOn DATETIME
	DECLARE @CreatedBy INT
	DECLARE @ModifiedOn DATETIME
	DECLARE @ModifiedBy INT
	DECLARE @default_unit TINYINT
	DECLARE @enginesize INT
	DECLARE @approved BIT
	DECLARE @exemptFromHomeToOffice BIT
	DECLARE @taxAttachID INT
	DECLARE @MOTAttachID INT
	DECLARE @insuranceAttachID INT
	DECLARE @serviceAttachID INT
	DECLARE @tmp TABLE (
		tmpID BIGINT
		,registration NVARCHAR(50)
		,carid INT
		)
     DECLARE @zeroCarIds TABLE (
	     registration NVARCHAR(50)
		,carid INT   
     )
     
	INSERT @tmp
	SELECT ROW_NUMBER() OVER (
			ORDER BY registration
			)
		,registration
		,carid
	FROM @list

	SELECT @count = count(*)
	FROM @tmp

	SET @index = 1

	WHILE @index <= @count
	BEGIN
		SET @registration = (
				SELECT TOP 1 registration
				FROM @tmp
				WHERE tmpID = @index
				);

		SELECT TOP 1 @carid = carid
			,@employeeid = employeeid
			,@startdate = startdate
			,@enddate = enddate
			,@make = make
			,@model = model
			,@registration = registration
			,@mileageid = mileageid
			,@cartypeid = cartypeid
			,@active = active
			,@odometer = odometer
			,@fuelcard = fuelcard
			,@endodometer = endodometer
			,@taxexpiry = taxexpiry
			,@taxlastchecked = taxlastchecked
			,@taxcheckedby = taxcheckedby
			,@mottestnumber = mottestnumber
			,@motlastchecked = motlastchecked
			,@motcheckedby = motcheckedby
			,@motexpiry = motexpiry
			,@insurancenumber = insurancenumber
			,@insuranceexpiry = insuranceexpiry
			,@insurancelastchecked = insurancelastchecked
			,@insurancecheckedby = insurancecheckedby
			,@serviceexpiry = serviceexpiry
			,@servicelastchecked = servicelastchecked
			,@servicecheckedby = servicecheckedby
			,@CreatedOn = CreatedOn
			,@CreatedBy = CreatedBy
			,@ModifiedOn = ModifiedOn
			,@ModifiedBy = ModifiedBy
			,@default_unit = default_unit
			,@enginesize = enginesize
			,@approved = approved
			,@exemptFromHomeToOffice = exemptFromHomeToOffice
			,@taxAttachID = taxAttachID
			,@MOTAttachID = MOTAttachID
			,@insuranceAttachID = insuranceAttachID
			,@serviceAttachID = serviceAttachID
		FROM @list
		WHERE registration = @registration

		IF @carid = 0
		BEGIN
			INSERT INTO [dbo].[cars] (
				[employeeid]
				,[startdate]
				,[enddate]
				,[make]
				,[model]
				,[registration]
				,[mileageid]
				,[cartypeid]
				,[active]
				,[odometer]
				,[fuelcard]
				,[endodometer]
				,[taxexpiry]
				,[taxlastchecked]
				,[taxcheckedby]
				,[mottestnumber]
				,[motlastchecked]
				,[motcheckedby]
				,[motexpiry]
				,[insurancenumber]
				,[insuranceexpiry]
				,[insurancelastchecked]
				,[insurancecheckedby]
				,[serviceexpiry]
				,[servicelastchecked]
				,[servicecheckedby]
				,[CreatedOn]
				,[CreatedBy]
				,[ModifiedOn]
				,[ModifiedBy]
				,[default_unit]
				,[enginesize]
				,[approved]
				,[exemptFromHomeToOffice]
				,[taxAttachID]
				,[MOTAttachID]
				,[insuranceAttachID]
				,[serviceAttachID]
				)
			VALUES (
				@employeeid
				,@startdate
				,@enddate
				,@make
				,@model
				,@registration
				,@mileageid
				,@cartypeid
				,@active
				,@odometer
				,@fuelcard
				,@endodometer
				,@taxexpiry
				,@taxlastchecked
				,@taxcheckedby
				,@mottestnumber
				,@motlastchecked
				,@motcheckedby
				,@motexpiry
				,@insurancenumber
				,@insuranceexpiry
				,@insurancelastchecked
				,@insurancecheckedby
				,@serviceexpiry
				,@servicelastchecked
				,@servicecheckedby
				,@CreatedOn
				,@CreatedBy
				,@ModifiedOn
				,@ModifiedBy
				,@default_unit
				,@enginesize
				,@approved
				,@exemptFromHomeToOffice
				,@taxAttachID
				,@MOTAttachID
				,@insuranceAttachID
				,@serviceAttachID
				)

			SET @carid = scope_identity();
			INSERT @zeroCarIds
				SELECT @registration, @carid;
		END
		ELSE
		BEGIN
			UPDATE [dbo].[cars]
			SET [employeeid] = @employeeid
				,[startdate] = @startdate
				,[enddate] = @enddate
				,[make] = @make
				,[model] = @model
				,[registration] = @registration
				,[mileageid] = @mileageid
				,[cartypeid] = @cartypeid
				,[active] = @active
				,[odometer] = @odometer
				,[fuelcard] = @fuelcard
				,[endodometer] = @endodometer
				,[taxexpiry] = @taxexpiry
				,[taxlastchecked] = @taxlastchecked
				,[taxcheckedby] = @taxcheckedby
				,[mottestnumber] = @mottestnumber
				,[motlastchecked] = @motlastchecked
				,[motcheckedby] = @motcheckedby
				,[motexpiry] = @motexpiry
				,[insurancenumber] = @insurancenumber
				,[insuranceexpiry] = @insuranceexpiry
				,[insurancelastchecked] = @insurancelastchecked
				,[insurancecheckedby] = @insurancecheckedby
				,[serviceexpiry] = @serviceexpiry
				,[servicelastchecked] = @servicelastchecked
				,[servicecheckedby] = @servicecheckedby
				,[CreatedOn] = @CreatedOn
				,[CreatedBy] = @CreatedBy
				,[ModifiedOn] = @ModifiedOn
				,[ModifiedBy] = @ModifiedBy
				,[default_unit] = @default_unit
				,[enginesize] = @enginesize
				,[approved] = @approved
				,[exemptFromHomeToOffice] = @exemptFromHomeToOffice
				,[taxAttachID] = @taxAttachID
				,[MOTAttachID] = @MOTAttachID
				,[insuranceAttachID] = @insuranceAttachID
				,[serviceAttachID] = @serviceAttachID
			WHERE carid = @carid
		END

		SET @index = @index + 1
	END

     SELECT * from @zeroCarIds;
     
	RETURN 0;
END
CREATE PROCEDURE [dbo].[saveCar]
	@carid int,
	@employeeid int,
	@startdate DateTime,
	@enddate DateTime,
	@make nvarchar(50),
	@model nvarchar(50),
	@registration nvarchar(50),
	@cartypeid tinyint,
	@active bit,
	@odometer bigint,
	@fuelcard bit,
	@endodometer int,
    @defaultunit tinyint,
	@enginesize int,
	@approved bit,
	@exemptFromHomeToOffice bit,
	@date DateTime,
	@userid int,
	@CUemployeeID INT,
	@CUdelegateID INT,
	@vehicletypeid tinyint,
	@isTaxvalid    bit,
	@isMotValid    bit,
	@taxExpiry	DateTime,
	@motExpiry	DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    if @carid = 0
		begin
			insert into cars (employeeid, make, model, registration, startdate, enddate, cartypeid, active, odometer, fuelcard, endodometer, default_unit, enginesize, approved, createdon, createdby, exemptFromHomeToOffice,vehicletypeid, taxexpiry, TaxValid, motexpiry, MotValid)
                values (@employeeid,@make,@model,@registration,@startdate,@enddate,@cartypeid,@active,@odometer,@fuelcard,@endodometer, @defaultunit, @engineSize, @approved, @date, @userid,@exemptFromHomeToOffice,@vehicletypeid, @taxexpiry, @istaxvalid, @motexpiry, @isMotValid);
			set @carid = scope_identity();

			if @CUemployeeID > 0
			BEGIN
				exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, @registration, null;
			END
		end
	else
		begin

			declare @oldstartdate DateTime;
			declare @oldenddate DateTime;
			declare @oldmake nvarchar(50);
			declare @oldmodel nvarchar(50);
			declare @oldregistration nvarchar(50);
			declare @oldcartypeid tinyint;
			declare @oldactive bit;
			declare @oldodometer bigint;
			declare @oldfuelcard bit;
			declare @oldendodometer int;
			declare @olddefaultunit tinyint;
			declare @oldenginesize int;
			declare @oldapproved bit;
			declare @oldexemptFromHomeToOffice bit;
			declare @oldvehicletypeid tinyint;

			select @oldstartdate = startdate, @oldenddate = enddate, @oldmake = make, @oldmodel = model, @oldregistration = registration, @oldcartypeid = cartypeid, @oldactive = active, @oldodometer = odometer, @oldfuelcard = fuelcard, @oldendodometer = endodometer, @olddefaultunit = default_unit, @oldenginesize = enginesize, @oldapproved = approved, @oldexemptFromHomeToOffice = exemptFromHomeToOffice, @oldvehicletypeid = vehicletypeid from cars where carid = @carid;

			update cars set make = @make, model = @model, registration = @registration, odometer = @odometer, startdate = @startdate, enddate = @enddate, cartypeid = @cartypeid, active = @active, fuelcard = @fuelcard, endodometer = @endodometer, default_unit = @defaultunit, enginesize=@engineSize, approved=@approved, modifiedon = @date, modifiedby = @userid, exemptFromHomeToOffice = @exemptFromHomeToOffice, vehicletypeid = @vehicletypeid, taxexpiry=@taxExpiry, TaxValid=@isTaxvalid, motexpiry=@motExpiry, MotValid=@isMotValid where carid = @carid;

					if @CUemployeeID > 0
			BEGIN
				if (@oldstartdate <> @startdate) or (@oldstartdate is NULL and @startdate is not NULL)
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'd226c1bd-ecc3-4f37-a5fe-58638b1bd66c', @oldstartdate, @startdate, @registration, NULL;
				if (@oldenddate <> @enddate) or (@oldenddate is NULL and @enddate is not NULL)
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '2ab21296-77ee-4b3d-807c-56edf936613d', @oldenddate, @enddate, @registration, NULL;
				if (@oldmake <> @make) or (@oldmake is NULL and @make is not NULL)
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'b7961f43-e439-4835-9709-396fff9bbd0c', @oldmake, @make, @registration, NULL;
				if (@oldmodel <> @model) or (@oldmodel is NULL and @model is not NULL)
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '99a078d9-f82c-4474-bdde-6701d4bd51ea', @oldmodel, @model, @registration, NULL;
				if (@oldregistration <> @registration) or (@oldregistration is NULL and @registration is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '156ccca7-1f5c-45be-920c-5e5c199ee81a', @oldregistration, @registration, @registration, NULL;
				if NOT EXISTS(SELECT @oldcartypeid INTERSECT SELECT @cartypeid) -- IF x IS DISTINCT FROM y, see http://stackoverflow.com/questions/10416789/
				begin
					declare @oldcartypeidAsString nvarchar(max);
					select @oldcartypeidAsString = Name
					from VehicleEngineTypes
					where VehicleEngineTypeId = @oldcartypeid;

					declare @cartypeidAsString nvarchar(max);
					select @cartypeidAsString = Name
					from VehicleEngineTypes
					where VehicleEngineTypeId = @cartypeid;

					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '24172542-3e15-4fca-b4f5-d7ffef9eed4e', @oldcartypeidAsString, @cartypeidAsString, @registration, NULL;
				end
				if (@oldactive <> @active) or (@oldactive is NULL and @active is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'dad8f087-497b-4a83-ab40-6b5b816911eb', @oldactive, @active, @registration, NULL;
				if (@oldodometer <> @odometer) or (@oldodometer is NULL and @odometer is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'DB940785-0B84-4B17-8C64-B370C25A145D', @oldodometer, @odometer, @registration, NULL;
				if (@oldfuelcard <> @fuelcard) or (@oldfuelcard is NULL and @fuelcard is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '9d3081c9-789d-49df-8764-1a3d4f32ae29', @oldfuelcard, @fuelcard, @registration, NULL;
				if (@oldendodometer <> @endodometer) or (@oldendodometer is NULL and @endodometer is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'cf725092-d2f0-48b0-a359-d8149750de81', @oldendodometer, @endodometer, @registration, NULL;					
				if (@olddefaultunit <> @defaultunit) or (@olddefaultunit is NULL and @defaultunit is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'f64ae656-cb64-4a9b-88fe-50d73538166a', @olddefaultunit, @defaultunit, @registration, NULL;
				if (@oldenginesize <> @enginesize) or (@oldenginesize is NULL and @enginesize is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '1f4e203c-1436-494e-9f53-9ff4fc6e2be3', @oldenginesize, @enginesize, @registration, NULL;
				if (@oldapproved <> @approved) or (@oldapproved is NULL and @approved is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '81f4cba4-c74d-449e-acd4-3ddd44d97a9b', @oldapproved, @approved, @registration, NULL;
				if (@oldexemptFromHomeToOffice) <> @exemptFromHomeToOffice or (@oldexemptFromHomeToOffice is NULL and @exemptFromHomeToOffice is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'B41F23CD-F766-4DDE-ADE3-399DFD647044', @oldexemptFromHomeToOffice, @exemptFromHomeToOffice, @registration, NULL;
			END
		end

	IF @employeeid <> 0
		UPDATE employees SET modifiedon = @date, modifiedby = @userid WHERE employeeid = @employeeid;
	return @carid;
	
END

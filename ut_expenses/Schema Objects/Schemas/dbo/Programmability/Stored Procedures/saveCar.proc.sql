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
	@taxexpiry DateTime,
	@taxlastchecked DateTime,
	@taxcheckedby int,
	@mottestnumber nvarchar(50),
	@motlastchecked DateTime,
	@motcheckedby int,
	@motexpiry DateTime,
	@insurancenumber nvarchar(50),
	@insuranceexpiry DateTime,
	@insurancelastchecked DateTime,
	@insurancecheckedby int,
	@serviceexpiry DateTime,
	@servicelastchecked DateTime,
	@servicecheckedby int,
	@defaultunit tinyint,
	@enginesize int,
	@approved bit,
	@exemptFromHomeToOffice bit,
	@date DateTime,
	@userid int,
	@CUemployeeID INT,
	@CUdelegateID INT,
	@vehicletypeid tinyint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    if @carid = 0
		begin
			insert into cars (employeeid, make, model, registration, startdate, enddate, cartypeid, active, odometer, fuelcard, endodometer, taxexpiry, taxlastchecked, taxcheckedby, motexpiry, motlastchecked, motcheckedby, mottestnumber, insuranceexpiry, insurancelastchecked, insurancecheckedby, insurancenumber, serviceexpiry, servicelastchecked, servicecheckedby, default_unit, enginesize, approved, createdon, createdby, exemptFromHomeToOffice,vehicletypeid)
                values (@employeeid,@make,@model,@registration,@startdate,@enddate,@cartypeid,@active,@odometer,@fuelcard,@endodometer,@taxexpiry,@taxlastchecked,@taxcheckedby,@motexpiry,@motlastchecked,@motcheckedby, @mottestnumber, @insuranceexpiry, @insurancelastchecked, @insurancecheckedby, @insurancenumber, @serviceexpiry, @servicelastchecked, @servicecheckedby, @defaultunit, @engineSize, @approved, @date, @userid,@exemptFromHomeToOffice,@vehicletypeid);
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
			declare @oldtaxexpiry DateTime;
			declare @oldtaxlastchecked DateTime;
			declare @oldtaxcheckedby int;
			declare @oldmottestnumber nvarchar(50);
			declare @oldmotlastchecked DateTime;
			declare @oldmotcheckedby int;
			declare @oldmotexpiry DateTime;
			declare @oldinsurancenumber nvarchar(50);
			declare @oldinsuranceexpiry DateTime;
			declare @oldinsurancelastchecked DateTime;
			declare @oldinsurancecheckedby int;
			declare @oldserviceexpiry DateTime;
			declare @oldservicelastchecked DateTime;
			declare @oldservicecheckedby int;
			declare @olddefaultunit tinyint;
			declare @oldenginesize int;
			declare @oldapproved bit;
			declare @oldexemptFromHomeToOffice bit;
			declare @oldvehicletypeid tinyint;

			select @oldstartdate = startdate, @oldenddate = enddate, @oldmake = make, @oldmodel = model, @oldregistration = registration, @oldcartypeid = cartypeid, @oldactive = active, @oldodometer = odometer, @oldfuelcard = fuelcard, @oldendodometer = endodometer, @oldtaxexpiry = taxexpiry, @oldtaxlastchecked = taxlastchecked, @oldtaxcheckedby = taxcheckedby, @oldmottestnumber = mottestnumber, @oldmotlastchecked = motlastchecked, @oldmotcheckedby = motcheckedby, @oldmotexpiry = motexpiry, @oldinsurancenumber = insurancenumber, @oldinsuranceexpiry = insuranceexpiry, @oldinsurancelastchecked = insurancelastchecked, @oldinsurancecheckedby = insurancecheckedby, @oldserviceexpiry = serviceexpiry, @oldservicelastchecked = servicelastchecked, @oldservicecheckedby = servicecheckedby, @olddefaultunit = default_unit, @oldenginesize = enginesize, @oldapproved = approved, @oldexemptFromHomeToOffice = exemptFromHomeToOffice, @oldvehicletypeid = vehicletypeid from cars where carid = @carid;

			update cars set make = @make, model = @model, registration = @registration, odometer = @odometer, startdate = @startdate, enddate = @enddate, cartypeid = @cartypeid, active = @active, fuelcard = @fuelcard, endodometer = @endodometer, taxexpiry = @taxexpiry, taxlastchecked = @taxlastchecked, taxcheckedby = @taxcheckedby, motexpiry = @motexpiry, motlastchecked = @motlastchecked, motcheckedby = @motcheckedby, mottestnumber = @mottestnumber, insuranceexpiry = @insuranceexpiry, insurancelastchecked = @insurancelastchecked, insurancecheckedby = @insurancecheckedby, insurancenumber = @insurancenumber, serviceexpiry = @serviceexpiry, servicelastchecked = @servicelastchecked, servicecheckedby = @servicecheckedby, default_unit = @defaultunit, enginesize=@engineSize, approved=@approved, modifiedon = @date, modifiedby = @userid, exemptFromHomeToOffice = @exemptFromHomeToOffice, vehicletypeid = @vehicletypeid where carid = @carid;

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
				if (@oldtaxexpiry <> @taxexpiry) or (@oldtaxexpiry is NULL and @taxexpiry is not NULL)
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '3e49160f-349a-466f-b9bf-6f1015b8415a', @oldtaxexpiry, @taxexpiry, @registration, NULL;
				if (@oldtaxlastchecked <> @taxlastchecked) or (@oldtaxlastchecked is NULL and @taxlastchecked is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '22f22e47-7756-4ceb-b2ce-9d7008be8e6a', @oldtaxlastchecked, @taxlastchecked, @registration, NULL;
				if (@oldtaxcheckedby <> @taxcheckedby) or (@oldtaxcheckedby is NULL and @taxcheckedby is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '184C7C86-4228-4B9D-805E-5FAEB0B42DE3', @oldtaxcheckedby, @taxcheckedby, @registration, NULL;
				if (@oldmottestnumber <> @mottestnumber) or (@oldmottestnumber is NULL and @mottestnumber is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '77a9770f-12c9-4966-98ce-6300ce5fbb57', @oldmottestnumber, @mottestnumber, @registration, NULL;
				if (@oldmotlastchecked <> @motlastchecked) or (@oldmotlastchecked is NULL and @motlastchecked is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'b1c98355-ae47-4bd1-b4a8-71622acc1b2f', @oldmotlastchecked, @motlastchecked, @registration, NULL;
				if (@oldmotcheckedby <> @motcheckedby) or (@oldmotcheckedby is NULL and @motcheckedby is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '082BCA3A-A172-4601-8CFE-52A4DDE3A16C', @oldmotcheckedby, @motcheckedby, @registration, NULL;
				if (@oldmotexpiry <> @motexpiry) or (@oldmotexpiry is NULL and @motexpiry is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'eedfb5e6-848b-4c5f-9cb9-20e20ace0f7d', @oldmotexpiry, @motexpiry, @registration, NULL;
				if (@oldinsurancenumber <> @insurancenumber) or (@oldinsurancenumber is NULL and @insurancenumber is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '5d90d3a4-40da-403b-b417-2ac445393a37', @oldinsurancenumber, @insurancenumber, @registration, NULL;
				if (@oldinsuranceexpiry <> @insuranceexpiry) or (@oldinsuranceexpiry is NULL and @insuranceexpiry is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'bab44a26-5690-4f3e-8003-206ee3fb673f', @oldinsuranceexpiry, @insuranceexpiry, @registration, NULL;
				if (@oldinsurancelastchecked <> @insurancelastchecked) or (@oldinsurancelastchecked is NULL and @insurancelastchecked is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '7c8b48f2-6299-400c-85c0-d6584232cddf', @oldinsurancelastchecked, @insurancelastchecked, @registration, NULL;
				if (@oldinsurancecheckedby <> @insurancecheckedby) or (@oldinsurancecheckedby is NULL and @insurancecheckedby is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'A8A75016-A8C6-4CFD-8487-FC661022749C', @oldinsurancecheckedby, @insurancecheckedby, @registration, NULL;
				if (@oldserviceexpiry <> @serviceexpiry) or (@oldserviceexpiry is NULL and @serviceexpiry is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'a380a705-c14b-4a07-a510-2f1a34fada98', @oldserviceexpiry, @serviceexpiry, @registration, NULL;
				if (@oldservicelastchecked <> @servicelastchecked) or (@oldservicelastchecked is NULL and @servicelastchecked is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '766682af-1fde-43f5-941d-8c5fcad1c7db', @oldservicelastchecked, @servicelastchecked, @registration, NULL;
				if (@oldservicecheckedby <> @servicecheckedby) or (@oldservicecheckedby is NULL and @servicecheckedby is not NULL) 
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '09F003F9-1F32-4CD9-A83E-2804AEA72F8B', @oldservicecheckedby, @servicecheckedby, @registration, NULL;
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
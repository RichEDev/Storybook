


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
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
	@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    if @carid = 0
		begin
			insert into cars (employeeid, make, model, registration, startdate, enddate, cartypeid, active, odometer, fuelcard, endodometer, taxexpiry, taxlastchecked, taxcheckedby, motexpiry, motlastchecked, motcheckedby, mottestnumber, insuranceexpiry, insurancelastchecked, insurancecheckedby, insurancenumber, serviceexpiry, servicelastchecked, servicecheckedby, default_unit, enginesize, approved, createdon, createdby, exemptFromHomeToOffice)
                values (@employeeid,@make,@model,@registration,@startdate,@enddate,@cartypeid,@active,@odometer,@fuelcard,@endodometer,@taxexpiry,@taxlastchecked,@taxcheckedby,@motexpiry,@motlastchecked,@motcheckedby, @mottestnumber, @insuranceexpiry, @insurancelastchecked, @insurancecheckedby, @insurancenumber, @serviceexpiry, @servicelastchecked, @servicecheckedby, @defaultunit, @engineSize, @approved, @date, @userid,@exemptFromHomeToOffice);
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
			select @oldstartdate = startdate, @oldenddate = enddate, @oldmake = make, @oldmodel = model, @oldregistration = registration, @oldcartypeid = cartypeid, @oldactive = active, @oldodometer = odometer, @oldfuelcard = fuelcard, @oldendodometer = endodometer, @oldtaxexpiry = taxexpiry, @oldtaxlastchecked = taxlastchecked, @oldtaxcheckedby = taxcheckedby, @oldmottestnumber = mottestnumber, @oldmotlastchecked = motlastchecked, @oldmotcheckedby = motcheckedby, @oldmotexpiry = motexpiry, @oldinsurancenumber = insurancenumber, @oldinsuranceexpiry = insuranceexpiry, @oldinsurancelastchecked = insurancelastchecked, @oldinsurancecheckedby = insurancecheckedby, @oldserviceexpiry = serviceexpiry, @oldservicelastchecked = servicelastchecked, @oldservicecheckedby = servicecheckedby, @olddefaultunit = default_unit, @oldenginesize = enginesize, @oldapproved = approved, @oldexemptFromHomeToOffice = exemptFromHomeToOffice from cars where carid = @carid;

			update cars set make = @make, model = @model, registration = @registration, odometer = @odometer, startdate = @startdate, enddate = @enddate, cartypeid = @cartypeid, active = @active, fuelcard = @fuelcard, endodometer = @endodometer, taxexpiry = @taxexpiry, taxlastchecked = @taxlastchecked, taxcheckedby = @taxcheckedby, motexpiry = @motexpiry, motlastchecked = @motlastchecked, motcheckedby = @motcheckedby, mottestnumber = @mottestnumber, insuranceexpiry = @insuranceexpiry, insurancelastchecked = @insurancelastchecked, insurancecheckedby = @insurancecheckedby, insurancenumber = @insurancenumber, serviceexpiry = @serviceexpiry, servicelastchecked = @servicelastchecked, servicecheckedby = @servicecheckedby, default_unit = @defaultunit, enginesize=@engineSize, approved=@approved, modifiedon = @date, modifiedby = @userid, exemptFromHomeToOffice = @exemptFromHomeToOffice where carid = @carid;

			if @CUemployeeID > 0
			BEGIN
				if @oldstartdate <> @startdate
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'd226c1bd-ecc3-4f37-a5fe-58638b1bd66c', @oldstartdate, @startdate, @registration, null;
				if @oldenddate <> @enddate
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '2ab21296-77ee-4b3d-807c-56edf936613d', @oldenddate, @enddate, @registration, null;
				if @oldmake <> @make
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'b7961f43-e439-4835-9709-396fff9bbd0c', @oldmake, @make, @registration, null;
				if @oldmodel <> @model
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '99a078d9-f82c-4474-bdde-6701d4bd51ea', @oldmodel, @model, @registration, null;
				if @oldregistration <> @registration
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '156ccca7-1f5c-45be-920c-5e5c199ee81a', @oldregistration, @registration, @registration, null;
				if @oldcartypeid <> @cartypeid
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '24172542-3e15-4fca-b4f5-d7ffef9eed4e', @oldcartypeid, @cartypeid, @registration, null;
				if @oldactive <> @active
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'dad8f087-497b-4a83-ab40-6b5b816911eb', @oldactive, @active, @registration, null;
				if @oldodometer <> @odometer
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, null, @oldodometer, @odometer, @registration, null;
				if @oldfuelcard <> @fuelcard
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '9d3081c9-789d-49df-8764-1a3d4f32ae29', @oldfuelcard, @fuelcard, @registration, null;
				if @oldendodometer <> @endodometer
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'cf725092-d2f0-48b0-a359-d8149750de81', @oldendodometer, @endodometer, @registration, null;
				if @oldtaxexpiry <> @taxexpiry
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '3e49160f-349a-466f-b9bf-6f1015b8415a', @oldtaxexpiry, @taxexpiry, @registration, null;
				if @oldtaxlastchecked <> @taxlastchecked
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '22f22e47-7756-4ceb-b2ce-9d7008be8e6a', @oldtaxlastchecked, @taxlastchecked, @registration, null;
				if @oldtaxcheckedby <> @taxcheckedby
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, null, @oldtaxcheckedby, @taxcheckedby, @registration, null;
				if @oldmottestnumber <> @mottestnumber
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '77a9770f-12c9-4966-98ce-6300ce5fbb57', @oldmottestnumber, @mottestnumber, @registration, null;
				if @oldmotlastchecked <> @motlastchecked
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'b1c98355-ae47-4bd1-b4a8-71622acc1b2f', @oldmotlastchecked, @motlastchecked, @registration, null;
				if @oldmotcheckedby <> @motcheckedby
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, null, @oldmotcheckedby, @motcheckedby, @registration, null;
				if @oldmotexpiry <> @motexpiry
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'eedfb5e6-848b-4c5f-9cb9-20e20ace0f7d', @oldmotexpiry, @motexpiry, @registration, null;
				if @oldinsurancenumber <> @insurancenumber
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '5d90d3a4-40da-403b-b417-2ac445393a37', @oldinsurancenumber, @insurancenumber, @registration, null;
				if @oldinsuranceexpiry <> @insuranceexpiry
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'bab44a26-5690-4f3e-8003-206ee3fb673f', @oldinsuranceexpiry, @insuranceexpiry, @registration, null;
				if @oldinsurancelastchecked <> @insurancelastchecked
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '7c8b48f2-6299-400c-85c0-d6584232cddf', @oldinsurancelastchecked, @insurancelastchecked, @registration, null;
				if @oldinsurancecheckedby <> @insurancecheckedby
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, null, @oldinsurancecheckedby, @insurancecheckedby, @registration, null;
				if @oldserviceexpiry <> @serviceexpiry
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'a380a705-c14b-4a07-a510-2f1a34fada98', @oldserviceexpiry, @serviceexpiry, @registration, null;
				if @oldservicelastchecked <> @servicelastchecked
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '766682af-1fde-43f5-941d-8c5fcad1c7db', @oldservicelastchecked, @servicelastchecked, @registration, null;
				if @oldservicecheckedby <> @servicecheckedby
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, null, @oldservicecheckedby, @servicecheckedby, @registration, null;
				if @olddefaultunit <> @defaultunit
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, 'f64ae656-cb64-4a9b-88fe-50d73538166a', @olddefaultunit, @defaultunit, @registration, null;
				if @oldenginesize <> @enginesize
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '1f4e203c-1436-494e-9f53-9ff4fc6e2be3', @oldenginesize, @enginesize, @registration, null;
				if @oldapproved <> @approved
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, '81f4cba4-c74d-449e-acd4-3ddd44d97a9b', @oldapproved, @approved, @registration, null;
				if @oldexemptFromHomeToOffice <> @exemptFromHomeToOffice
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, null, @oldexemptFromHomeToOffice, @exemptFromHomeToOffice, @registration, null;
			END
		end

	IF @employeeid <> 0
		UPDATE employees SET modifiedon = @date, modifiedby = @userid WHERE employeeid = @employeeid;
	return @carid;
	
END



 

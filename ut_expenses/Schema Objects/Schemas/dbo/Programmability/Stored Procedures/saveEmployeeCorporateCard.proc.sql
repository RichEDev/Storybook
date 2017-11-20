CREATE PROCEDURE [dbo].[saveEmployeeCorporateCard]
	@corporatecardid int,
	@employeeid int,
	@cardproviderid int,
	@cardnumber nvarchar(50),
	@active bit,
	@date DateTime,
	@userid int,
	@CUemployeeID INT,
	@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @recordTitle nvarchar(2000);

	if @corporatecardid = 0
		begin
			if exists (select * from employee_corporate_cards where cardnumber = @cardnumber)
			begin
				return -1;
			end
			
			insert into employee_corporate_cards (employeeid, cardproviderid, cardnumber, active, createdon, createdby) values (@employeeid, @cardproviderid, @cardnumber, @active, @date, @userid)
			set @corporatecardid = scope_identity();

			set @recordTitle = (select 'Corporate Card Number ' + @cardnumber);
			
			if @CUemployeeID > 0
			BEGIN
				exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 17, @corporatecardid, @recordTitle, null;
			END
		end
	else
		begin
			declare @oldcardproviderid int;
			declare @oldcardnumber nvarchar(50);
			declare @oldactive bit;
			select @oldcardproviderid = cardproviderid, @oldcardnumber = cardnumber, @oldactive = active from employee_corporate_cards where corporatecardid = @corporatecardid;

			update employee_corporate_cards set cardnumber = @cardnumber, active = @active, cardproviderid = @cardproviderid, modifiedon = @date, modifiedby = @userid where corporatecardid = @corporatecardid;

			set @recordTitle = (select 'Corporate Card Number ' + @cardnumber);

			if @CUemployeeID > 0
			BEGIN
				if @oldcardproviderid <> @cardproviderid
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 17, @corporatecardid, 'c1a1f34a-6fc3-44c1-8a19-61be80fb46e5', @oldcardproviderid, @cardproviderid, @recordtitle, null;
				if @oldcardnumber <> @cardnumber
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 17, @corporatecardid, '363ba78c-1077-41bb-a9ed-e761cd50d58d', @oldcardnumber, @cardnumber, @recordtitle, null;
				if @oldactive <> @active
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 17, @corporatecardid, '3a14e819-108e-41a0-97a4-297bd684d346', @oldactive, @active, @recordtitle, null;
			END
		END
		
		UPDATE employees SET modifiedon = @date, modifiedby = @userid WHERE employeeid = @employeeid
	return @corporatecardid
END

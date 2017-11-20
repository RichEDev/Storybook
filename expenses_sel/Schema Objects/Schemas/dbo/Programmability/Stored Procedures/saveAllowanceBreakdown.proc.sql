

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[saveAllowanceBreakdown]
	-- Add the parameters for the stored procedure here
	@breakdownid int,
	@allowanceid int,
	@hours int,
	@rate money,
	@delegateID INT,
	@employeeID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @recordTitle nvarchar(2000);

    -- Insert statements for procedure here
	if @breakdownid = 0
		begin
			insert into allowancebreakdown (allowanceid, hours, rate) values (@allowanceid, @hours, @rate);
			set @breakdownid = scope_identity();

			select @recordTitle = allowance from allowances where allowanceid = (select allowanceid from allowancebreakdown where breakdownid = @breakdownid);

			exec addInsertEntryToAuditLog @employeeID, @delegateID, 8, @breakdownid, @recordTitle, null;
		end
	else
		begin
			declare @oldHours int
			declare @oldRate money
			select @oldHours = hours, @oldRate = rate from allowancebreakdown where breakdownid = @breakdownid;
			select @recordTitle = allowance from allowances where allowanceid = (select allowanceid from allowancebreakdown where breakdownid = @breakdownid);

			update allowancebreakdown set hours = @hours, rate = @rate where breakdownid = @breakdownid;
			
			if @oldHours <> @hours
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 8, @breakdownid, 'ef2f0292-6437-4e86-a9bf-076074e5aad1', @oldHours, @hours, @recordTitle, null;
			if @oldRate <> @rate
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 8, @breakdownid, 'ccd12c98-7f07-4266-97e5-1444460b19bf', @oldRate, @rate, @recordTitle, null;
		end

	return @breakdownid;
	
END

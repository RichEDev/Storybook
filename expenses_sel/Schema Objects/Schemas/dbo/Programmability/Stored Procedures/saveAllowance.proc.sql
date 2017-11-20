

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[saveAllowance]
	-- Add the parameters for the stored procedure here
	@allowanceid int,
	@allowance nvarchar(50),
	@description nvarchar(4000),
	@nighthours int,
	@nightrate int,
	@currencyid int,
	@date DateTime,
	@userid int,
	@delegateID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @count int
    if @allowanceid = 0
		begin
			set @count = (select count(*) from allowances where allowance = @allowance);
			if @count > 0
				return -1;

			insert into allowances (allowance, description, nighthours, nightrate, currencyid, createdon, createdby) values (@allowance, @description,@nighthours,@nightrate, @currencyid, @date, @userid);
			set @allowanceid = scope_identity()

			exec addInsertEntryToAuditLog @userid, @delegateID, 8, @allowanceid, @allowance, null;
		end
	else
		begin
			set @count = (select count(*) from allowances where allowance = @allowance and allowanceid <> @allowanceid);
			if @count > 0
				return -1;

			declare @oldallowance nvarchar(4000);
			declare @olddescription nvarchar(4000);
			declare @oldnighthours int;
			declare @oldnightrate int;
			declare @oldcurrencyid int;
			select @oldallowance = allowance, @olddescription = description, @oldnighthours = nighthours, @oldnightrate = nightrate, @oldcurrencyid = currencyid from allowances where allowanceid = @allowanceid;

			update allowances set allowance = @allowance, description = @description, nighthours = @nighthours, nightrate = @nightrate, currencyid = @currencyid, modifiedon = @date, modifiedby = @userid where allowanceid = @allowanceid;
			
			if @oldallowance <> @allowance
				exec addUpdateEntryToAuditLog @userid, @delegateID, 8, @allowanceid, '74bec6a1-5520-46bc-96d1-759200bc206f', @oldallowance, @allowance, @allowance, null;
			if @olddescription <> @description
				exec addUpdateEntryToAuditLog @userid, @delegateID, 8, @allowanceid, 'aa264826-7f53-47be-9e20-0c467b9297e8', @olddescription, @description, @allowance, null;
			if @oldnighthours <> @nighthours
				exec addUpdateEntryToAuditLog @userid, @delegateID, 8, @allowanceid, '9aacd4a4-7b74-4167-a92b-518667d71b18', @oldnighthours, @nighthours, @allowance, null;
			if @oldnightrate <> @nightrate
				exec addUpdateEntryToAuditLog @userid, @delegateID, 8, @allowanceid, 'd8176644-f2f4-4d4d-b4ec-14b10d1b7c7a', @oldnightrate, @nightrate, @allowance, null;
			if @oldcurrencyid <> @currencyid
				exec addUpdateEntryToAuditLog @userid, @delegateID, 8, @allowanceid, '090b601a-1899-416a-82d5-48bf175d68e0', @oldcurrencyid, @currencyid, @allowance, null;
		end

	return @allowanceid;
END



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[saveRoleSubcat]
	@roleid int,
	@subcatid int,
	@isadditem bit,
	@maximum money,
	@receiptmaximum money,
	@employeeID INT,
	@delegateID INT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @count int;
	declare @rolename varchar(500);
	declare @subcatname varchar(500);
	declare @recordTitle nvarchar(2000);

	select @rolename = rolename from item_roles where itemroleid = @roleid;
	select @subcatname = subcat from subcats where subcatid = @subcatid;
	set @recordTitle = (select 'Item Role ' + @rolename + ' - Subcat ' + @subcatname);
	
	set @count = (select count(*) from rolesubcats where roleid = @roleid and subcatid = @subcatid);

	if @count = 0
		begin
			insert into rolesubcats (roleid, subcatid, maximum, receiptmaximum, isadditem) values (@roleid,@subcatid,@maximum,@receiptmaximum,@isadditem);

			exec addInsertEntryToAuditLog @employeeID, @delegateID, 37, @roleid, @recordTitle, null;
		end
	else
		begin

			declare @oldisadditem bit;
			declare @oldmaximum money;
			declare @oldreceiptmaximum money;
			select @oldisadditem = isadditem, @oldmaximum = maximum, @oldreceiptmaximum = receiptmaximum from rolesubcats where roleid = @roleid and subcatid = @subcatid;

			update rolesubcats set isadditem = @isadditem, maximum = @maximum, receiptmaximum = @receiptmaximum where roleid = @roleid and subcatid = @subcatid;

			if @oldisadditem <> @isadditem
				begin
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, 37, @roleid, 'd2702ae8-609a-45ab-bf01-c58210ef1720', @oldisadditem, @isadditem, @recordtitle, null;
				end
			if @oldmaximum <> @maximum
				begin
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, 37, @roleid, '10a310fa-d34a-4568-b573-07a91f9aa765', @oldmaximum, @maximum, @recordtitle, null;
				end
			if @oldreceiptmaximum <> @receiptmaximum
				begin
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, 37, @roleid, 'a3ba1781-2c99-48dc-bc8c-126ef90ca55b', @oldreceiptmaximum, @receiptmaximum, @recordtitle, null;
				end
		end
END



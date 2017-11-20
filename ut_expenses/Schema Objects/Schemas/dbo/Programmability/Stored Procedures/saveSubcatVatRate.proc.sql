






-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[saveSubcatVatRate] (@vatrateid int, @subcatid int, @vatamount float, @vatreceipt bit, @vatpercent tinyint, @vatlimitwithout decimal, @vatlimitwith decimal, @daterangetype tinyint, @datevalue1 DateTime, @datevalue2 DateTime, @employeeID INT, @delegateID INT)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @title1 nvarchar(500);
	declare @recordTitle nvarchar(2000);

	select @title1 = subcat from subcats where subcatid = @subcatid;

    -- Insert statements for procedure here
	if @vatrateid = 0
		begin
			insert into subcat_vat_rates (subcatid, vatamount, vatreceipt, vatpercent, vatlimitwithout, vatlimitwith, daterangetype, datevalue1, datevalue2) values (@subcatid, @vatamount, @vatreceipt, @vatpercent, @vatlimitwithout, @vatlimitwith, @daterangetype, @datevalue1, @datevalue2);
			set @vatrateid = scope_identity();

			set @recordTitle = (select @title1 + ' vat rate ' + CAST(@vatrateid AS nvarchar(20)));
			exec addInsertEntryToAuditLog @employeeID, @delegateID, 29, @vatrateid, @recordTitle, null;
		end
	else
		begin
			declare @oldvatamount float;
			declare @oldvatreceipt bit;
			declare @oldvatpercent tinyint;
			declare @oldvatlimitwithout decimal;
			declare @oldvatlimitwith decimal;
			declare @olddaterangetype tinyint;
			declare @olddatevalue1 DateTime;
			declare @olddatevalue2 DateTime;

			select @oldvatamount = vatamount, @oldvatreceipt = vatreceipt, @oldvatpercent = vatpercent, @oldvatlimitwithout = vatlimitwithout, @oldvatlimitwith = vatlimitwith, @olddaterangetype = daterangetype, @olddatevalue1 = datevalue1, @olddatevalue2 = datevalue2 from subcat_vat_rates where vatrateid = @vatrateid

			update subcat_vat_rates set vatamount = @vatamount, vatreceipt = @vatreceipt, vatpercent = @vatpercent, vatlimitwithout = @vatlimitwithout, vatlimitwith = @vatlimitwith, daterangetype = @daterangetype, datevalue1 = @datevalue1, datevalue2 = @datevalue2 where vatrateid = @vatrateid

			set @recordTitle = (select @title1 + ' vat rate ' + CAST(@vatrateid AS nvarchar(20)));

			if @oldvatamount <> @vatamount
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @vatrateid, 'b616aacc-4875-4e35-8822-84c3d66505a3', @oldvatamount, @vatamount, @recordtitle, null;
			if @oldvatreceipt <> @vatreceipt
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @vatrateid, 'e9b695f1-c785-4091-93cf-973131deb95f', @oldvatreceipt, @vatreceipt, @recordtitle, null;
			if @oldvatpercent <> @vatpercent
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @vatrateid, '720bb087-a00b-491c-ab58-23307c1deb27', @oldvatpercent, @vatpercent, @recordtitle, null;
			if @oldvatlimitwithout <> @vatlimitwithout
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @vatrateid, '9ab7d9d0-4ea0-4729-b946-842f43f9be32', @oldvatlimitwithout, @vatlimitwithout, @recordtitle, null;
			if @oldvatlimitwith <> @vatlimitwith
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @vatrateid, 'cb826f5f-b6c4-4d4c-88a5-ace0dc9b4a35', @oldvatlimitwith, @vatlimitwith, @recordtitle, null;
			if @olddaterangetype <> @daterangetype
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @vatrateid, 'e534420c-3dae-48cb-8aca-15d32c84c23e', @olddaterangetype, @daterangetype, @recordtitle, null;
			if @olddatevalue1 <> @datevalue1
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @vatrateid, '3e60e0ed-b1bd-4d57-8653-cefb6a0d42d4', @olddatevalue1, @datevalue1, @recordtitle, null;
			if @olddatevalue2 <> @datevalue2
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @vatrateid, '85b2b764-40b0-4c2d-aef8-b9ea5168804c', @olddatevalue2, @datevalue2, @recordtitle, null;
		end

	return @vatrateid;
END







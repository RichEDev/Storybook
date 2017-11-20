
CREATE PROCEDURE [dbo].[saveSubcat]
@subcatid int,
@categoryid int,
@description nvarchar(4000),
@subcat nvarchar(50),
@mileageapp bit,
@staffapp bit,
@othersapp bit,
@tipapp bit,
@pmilesapp bit,
@bmilesapp bit,
@allowanceamount money,
@accountcode nvarchar(50),
@attendeesapp bit,
@addasnet bit,
@pdcatid int,
@eventinhomeapp bit,
@receiptapp bit,
@calculation tinyint,
@passengersapp bit,
@nopassengersapp bit,
@comment nvarchar(4000),
@splitentertainment bit,
@entertainmentid int,
@reimbursable bit,
@nonightsapp bit,
@attendeesmand bit,
@nodirectorsapp bit,
@hotelapp bit,
@noroomsapp bit,
@hotelmand bit,
@vatnumberapp bit,
@vatnumbermand bit,
@nopersonalguestsapp bit,
@noremoteworkersapp bit,
@alternateaccountcode nvarchar(50),
@splitpersonal bit,
@splitremote bit,
@personalid int,
@remoteid int,
@reasonapp bit,
@otherdetailsapp bit,
@shortsubcat nvarchar(50),
@fromapp bit,
@toapp bit,
@companyapp BIT,
@enableHomeToLocationMileage bit,
@homeToLocationType tinyint,
@mileageCategory int,
@isRelocationMileage bit,
@reimbursableSubcatID int,
@allowHeavyBulkyMileage bit,
@date DateTime,
@userid int,
@employeeID INT,
@delegateID INT

AS
declare @id int;
declare @count int
if (@subcatid = 0)
begin
	SET @count = (SELECT COUNT(*) FROM subcats WHERE subcat = @subcat);
	IF @count > 0
		RETURN -1;

	insert into subcats (categoryid, subcat, description, mileageapp, staffapp, othersapp, tipapp, pmilesapp, bmilesapp, calculation, allowanceamount, accountcode, attendeesapp, addasnet, pdcatid, eventinhomeapp, receiptapp, passengersapp, nopassengersapp, comment, splitentertainment, entertainmentid, reimbursable, nonightsapp, attendeesmand, nodirectorsapp, hotelapp, noroomsapp, hotelmand, vatnumberapp, vatnumbermand, nopersonalguestsapp, noremoteworkersapp, alternateaccountcode, splitpersonal, splitremote, personalid, remoteid, reasonapp, otherdetailsapp, createdon, createdby, shortsubcat, fromapp, toapp, companyapp, enableHomeToLocationMileage, homeToLocationType, mileageCategory, isRelocationMileage, reimbursableSubcatID, allowHeavyBulkyMileage)
				values (@categoryid,@subcat,@description,@mileageapp,@staffapp,@othersapp,@tipapp,@pmilesapp,@bmilesapp,@calculation, @allowanceamount,@accountcode,@attendeesapp,@addasnet,@pdcatid,@eventinhomeapp,@receiptapp,@passengersapp,@nopassengersapp,@comment, @splitentertainment, @entertainmentid, @reimbursable,@nonightsapp,@attendeesmand, @nodirectorsapp, @hotelapp, @noroomsapp, @hotelmand, @vatnumberapp, @vatnumbermand, @nopersonalguestsapp, @noremoteworkersapp, @alternateaccountcode, @splitpersonal, @splitremote, @personalid, @remoteid, @reasonapp, @otherdetailsapp, @date, @userid, @shortsubcat, @fromapp, @toapp, @companyapp, @enableHomeToLocationMileage, @homeToLocationType, @mileageCategory, @isRelocationMileage, @reimbursableSubcatID, @allowHeavyBulkyMileage)
	set @id = scope_identity();

	if @employeeID > 0
	BEGIN
		exec addInsertEntryToAuditLog @employeeID, @delegateID, 29, @id, @subcat, null;
	END
end
else
begin
	SET @count = (SELECT COUNT(*) FROM subcats WHERE subcat = @subcat and subcatid <> @subcatid);
	IF @count > 0
		RETURN -1;

	declare @oldcategoryid int;
	declare @olddescription nvarchar(4000);
	declare @oldsubcat nvarchar(50);
	declare @oldmileageapp bit;
	declare @oldstaffapp bit;
	declare @oldothersapp bit;
	declare @oldtipapp bit;
	declare @oldpmilesapp bit;
	declare @oldbmilesapp bit;
	declare @oldallowanceamount money;
	declare @oldaccountcode nvarchar(50);
	declare @oldattendeesapp bit;
	declare @oldaddasnet bit;
	declare @oldpdcatid int;
	declare @oldeventinhomeapp bit;
	declare @oldreceiptapp bit;
	declare @oldcalculation tinyint;
	declare @oldpassengersapp bit;
	declare @oldnopassengersapp bit;
	declare @oldcomment nvarchar(4000);
	declare @oldsplitentertainment bit;
	declare @oldentertainmentid int;
	declare @oldreimbursable bit;
	declare @oldnonightsapp bit;
	declare @oldattendeesmand bit;
	declare @oldnodirectorsapp bit;
	declare @oldhotelapp bit;
	declare @oldnoroomsapp bit;
	declare @oldhotelmand bit;
	declare @oldvatnumberapp bit;
	declare @oldvatnumbermand bit;
	declare @oldnopersonalguestsapp bit;
	declare @oldnoremoteworkersapp bit;
	declare @oldalternateaccountcode nvarchar(50);
	declare @oldsplitpersonal bit;
	declare @oldsplitremote bit;
	declare @oldpersonalid int;
	declare @oldremoteid int;
	declare @oldreasonapp bit;
	declare @oldotherdetailsapp bit;
	declare @oldshortsubcat nvarchar(50);
	declare @oldfromapp bit;
	declare @oldtoapp bit;
	declare @oldcompanyapp BIT;
	declare @oldenableHomeToLocationMileage bit;
	declare @oldhomeToLocationType tinyint;
	declare @oldmileageCategory int;
	declare @oldisRelocationMileage bit;
	declare @oldreimbursableSubcatID int;
	declare @oldAllowHeavyBulkyMileage bit;

	select @oldcategoryid = categoryid, @olddescription = description, @oldsubcat = subcat, @oldmileageapp = mileageapp, @oldstaffapp = staffapp, @oldothersapp = othersapp, @oldtipapp = tipapp, @oldpmilesapp = pmilesapp, @oldbmilesapp = bmilesapp, @oldallowanceamount = allowanceamount, @oldaccountcode = accountcode, @oldattendeesapp = attendeesapp, @oldaddasnet = addasnet, @oldpdcatid = pdcatid, @oldeventinhomeapp = eventinhomeapp, @oldreceiptapp = receiptapp, @oldcalculation = calculation, @oldpassengersapp = passengersapp, @oldnopassengersapp = nopassengersapp, @oldcomment = comment, @oldsplitentertainment = splitentertainment, @oldentertainmentid = entertainmentid, @oldreimbursable = reimbursable, @oldnonightsapp = nonightsapp, @oldattendeesmand = attendeesmand, @oldnodirectorsapp = nodirectorsapp, @oldhotelapp = hotelapp, @oldnoroomsapp = noroomsapp, @oldhotelmand = hotelmand, @oldvatnumberapp = vatnumberapp, @oldvatnumbermand = vatnumbermand, @oldnopersonalguestsapp = nopersonalguestsapp, @oldnoremoteworkersapp = noremoteworkersapp, @oldalternateaccountcode = alternateaccountcode, @oldsplitpersonal = splitpersonal, @oldsplitremote = splitremote, @oldpersonalid = personalid, @oldremoteid = remoteid, @oldreasonapp = reasonapp, @oldotherdetailsapp = otherdetailsapp, @oldshortsubcat = shortsubcat, @oldfromapp = fromapp, @oldtoapp = toapp, @oldcompanyapp = companyapp, @oldenableHomeToLocationMileage = enableHomeToLocationMileage, @oldhomeToLocationType = homeToLocationType, @oldmileageCategory = mileageCategory, @oldisRelocationMileage = isRelocationMileage, @oldreimbursableSubcatID = reimbursableSubcatID, @oldAllowHeavyBulkyMileage = allowHeavyBulkyMileage from subcats where subcatid = @subcatid

	update subcats set categoryid = @categoryid, pdcatid = @pdcatid, subcat = @subcat, accountcode = @accountcode, description = @description, calculation = @calculation, addasnet = @addasnet, allowanceamount = @allowanceamount, mileageapp = @mileageapp, staffapp = @staffapp, othersapp = @othersapp, attendeesapp = @attendeesapp, pmilesapp = @pmilesapp, bmilesapp = @bmilesapp, tipapp = @tipapp, eventinhomeapp = @eventinhomeapp, receiptapp = @receiptapp, splitentertainment = @splitentertainment, entertainmentid = @entertainmentid, reimbursable = @reimbursable, nonightsapp = @nonightsapp, nodirectorsapp = @nodirectorsapp
			, passengersapp = @passengersapp, nopassengersapp = @nopassengersapp, comment = @comment, attendeesmand = @attendeesmand, hotelapp = @hotelapp, noroomsapp = @noroomsapp, hotelmand = @hotelmand, vatnumberapp = @vatnumberapp, vatnumbermand = @vatnumbermand, nopersonalguestsapp = @nopersonalguestsapp, noremoteworkersapp = @noremoteworkersapp, alternateaccountcode = @alternateaccountcode, splitpersonal = @splitpersonal, splitremote = @splitremote, personalid = @personalid, remoteid = @remoteid, reasonapp = @reasonapp, otherdetailsapp = @otherdetailsapp, modifiedon = @date, modifiedby = @userid, shortsubcat = @shortsubcat, fromapp = @fromapp, toapp = @toapp, companyapp = @companyapp, enableHomeToLocationMileage = @enableHomeToLocationMileage, homeToLocationType = @homeToLocationType, mileageCategory = @mileageCategory, isRelocationMileage = @isRelocationMileage, reimbursableSubcatID = @reimbursableSubcatID, allowHeavyBulkyMileage = @allowHeavyBulkyMileage
			 where subcatid = @subcatid;
	set @id = @subcatid;

	if @employeeID > 0
	BEGIN
		if @oldcategoryid <> @categoryid
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, '8d75ba33-f648-42b6-ba4f-c543764677e0', @oldcategoryid, @categoryid, @subcat, null;
		if @olddescription <> @description
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, '379b67dd-654e-43cd-b55d-b9b5262eeeee', @olddescription, @description, @subcat, null;
		if @oldsubcat <> @subcat
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, 'abfe0bb2-e6ac-40d0-88ce-c5f7b043924d', @oldsubcat, @subcat, @subcat, null;
		if @oldmileageapp <> @mileageapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, 'cd6dbf07-88be-48e2-b981-bf6ab2a73b98', @oldmileageapp, @mileageapp, @subcat, null;
		if @oldstaffapp <> @staffapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, 'ec5c1cb4-c85b-43a5-8761-d28b89e53cec', @oldstaffapp, @staffapp, @subcat, null;
		if @oldothersapp <> @othersapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, 'ac96dd5f-c0e7-4395-9162-5b140b14d523', @oldothersapp, @othersapp, @subcat, null;
		if @oldtipapp <> @tipapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, '13c117de-3a28-429b-8d2a-c48cdc8d0283', @oldtipapp, @tipapp, @subcat, null;
		if @oldpmilesapp <> @pmilesapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, '654f0f11-b42b-46f1-9e75-ad978aee86b7', @oldpmilesapp, @pmilesapp, @subcat, null;
		if @oldbmilesapp <> @bmilesapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, '7d2c3ef1-9bcd-4821-90a4-f9b67090d438', @oldbmilesapp, @bmilesapp, @subcat, null;
		if @oldallowanceamount <> @allowanceamount
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, '2dd5b7fb-e9eb-479c-9098-21baad9bc4a9', @oldallowanceamount, @allowanceamount, @subcat, null;
		if @oldaccountcode <> @accountcode
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, '677ad491-39f6-4e2e-b033-7b84e02ccc91', @oldaccountcode, @accountcode, @subcat, null;
		if @oldattendeesapp <> @attendeesapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, '28f96eb7-a26b-4bae-b42a-b62f9dfd17b4', @oldattendeesapp, @attendeesapp, @subcat, null;
		if @oldaddasnet <> @addasnet
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, '72799670-0a03-45c7-8d26-a873e83f184b', @oldaddasnet, @addasnet, @subcat, null;
		if @oldpdcatid <> @pdcatid
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldpdcatid, @pdcatid, @subcat, null;
		if @oldeventinhomeapp <> @eventinhomeapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, '37896370-74d7-4f9e-86fd-1dbb836c2f5b', @oldeventinhomeapp, @eventinhomeapp, @subcat, null;
		if @oldreceiptapp <> @receiptapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, '5d863f96-2adc-4a0a-8dec-dae9e166ff86', @oldreceiptapp, @receiptapp, @subcat, null;
		if @oldcalculation <> @calculation
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldcalculation, @calculation, @subcat, null;
		if @oldpassengersapp <> @passengersapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldpassengersapp, @passengersapp, @subcat, null;
		if @oldnopassengersapp <> @nopassengersapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, 'e7407e46-235d-451e-9731-af43824903cf', @oldnopassengersapp, @nopassengersapp, @subcat, null;
		if @oldcomment <> @comment
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, '6fa903c4-d734-41f0-aff4-265dbf36ae5c', @oldcomment, @comment, @subcat, null;
		if @oldsplitentertainment <> @splitentertainment
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldsplitentertainment, @splitentertainment, @subcat, null;
		if @oldentertainmentid <> @entertainmentid
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldentertainmentid, @entertainmentid, @subcat, null;
		if @oldreimbursable <> @reimbursable
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, 'e30d846d-c9ca-4332-b139-80f20a95f28d', @oldreimbursable, @reimbursable, @subcat, null;
		if @oldnonightsapp <> @nonightsapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldnonightsapp, @nonightsapp, @subcat, null;
		if @oldattendeesmand <> @attendeesmand
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldattendeesmand, @attendeesmand, @subcat, null;
		if @oldnodirectorsapp <> @nodirectorsapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldnodirectorsapp, @nodirectorsapp, @subcat, null;
		if @oldhotelapp <> @hotelapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldhotelapp, @hotelapp, @subcat, null;
		if @oldnoroomsapp <> @noroomsapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldnoroomsapp, @noroomsapp, @subcat, null;
		if @oldhotelmand <> @hotelmand
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldhotelmand, @hotelmand, @subcat, null;
		if @oldvatnumberapp <> @vatnumberapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldvatnumberapp, @vatnumberapp, @subcat, null;
		if @oldvatnumbermand <> @vatnumbermand
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldvatnumbermand, @vatnumbermand, @subcat, null;
		if @oldnopersonalguestsapp <> @nopersonalguestsapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldnopersonalguestsapp, @nopersonalguestsapp, @subcat, null;
		if @oldnoremoteworkersapp <> @noremoteworkersapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldnoremoteworkersapp, @noremoteworkersapp, @subcat, null;
		if @oldalternateaccountcode <> @alternateaccountcode
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, 'd45066b0-5f47-4daa-b270-1c046e8702e2', @oldalternateaccountcode, @alternateaccountcode, @subcat, null;
		if @oldsplitpersonal <> @splitpersonal
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldsplitpersonal, @splitpersonal, @subcat, null;
		if @oldsplitremote <> @splitremote
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldsplitremote, @splitremote, @subcat, null;
		if @oldpersonalid <> @personalid
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldpersonalid, @personalid, @subcat, null;
		if @oldremoteid <> @remoteid
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldremoteid, @remoteid, @subcat, null;
		if @oldreasonapp <> @reasonapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldreasonapp, @reasonapp, @subcat, null;
		if @oldotherdetailsapp <> @otherdetailsapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldotherdetailsapp, @otherdetailsapp, @subcat, null;
		if @oldshortsubcat <> @shortsubcat
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldshortsubcat, @shortsubcat, @subcat, null;
		if @oldfromapp <> @fromapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldfromapp, @fromapp, @subcat, null;
		if @oldtoapp <> @toapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldtoapp, @toapp, @subcat, null;
		if @oldcompanyapp <> @companyapp
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldcompanyapp, @companyapp, @subcat, null;
		if @oldenableHomeToLocationMileage <> @enableHomeToLocationMileage
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldenableHomeToLocationMileage, @enableHomeToLocationMileage, @subcat, null;
		if @oldhomeToLocationType <> @homeToLocationType
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldhomeToLocationType, @homeToLocationType, @subcat, null;
		if @oldmileageCategory <> @mileageCategory
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldmileageCategory, @mileageCategory, @subcat, null;
		if @oldisRelocationMileage <> @isRelocationMileage
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldisRelocationMileage, @isRelocationMileage, @subcat, null;
		if @oldreimbursableSubcatID <> @reimbursableSubcatID
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldreimbursableSubcatID, @reimbursableSubcatID, @subcat, null;
		if @oldAllowHeavyBulkyMileage <> @allowHeavyBulkyMileage
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 29, @id, null, @oldAllowHeavyBulkyMileage, @allowHeavyBulkyMileage, @subcat, null;
	END
end

return @id

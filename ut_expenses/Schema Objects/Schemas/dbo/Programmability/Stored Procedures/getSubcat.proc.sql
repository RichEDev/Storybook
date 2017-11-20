CREATE PROCEDURE [dbo].[getSubcat]
	@subcatid int = null,
	@subcat nvarchar(50) = null
AS
	
	select 
		subcatid, categoryid, description, subcat, vatapp, vatamount, mileageapp, staffapp, othersapp, vatreceipt, tipapp, pmilesapp, bmilesapp,
		speedoapp, blitresapp, plitresapp, allowanceamount, accountcode, attendeesapp, addasnet, pdcatid, vatpercent, eventinhomeapp, receiptapp,
		calculation, passengersapp, nopassengersapp, passengernamesapp, comment, splitentertainment, entertainmentid, reimbursable, nonightsapp,
		attendeesmand, nodirectorsapp, hotelapp, noroomsapp, hotelmand, vatnumberapp, vatnumbermand, nopersonalguestsapp, noremoteworkersapp,
		alternateaccountcode, splitpersonal, splitremote, personalid, remoteid, vatlimitwithout, vatlimitwith, reasonapp, otherdetailsapp, CreatedOn,
		CreatedBy, ModifiedOn, ModifiedBy, shortsubcat, fromapp, toapp, companyapp, enableHomeToLocationMileage, homeToLocationType, mileageCategory,
		isRelocationMileage, reimbursableSubcatID, allowHeavyBulkyMileage, homeToOfficeAlwaysZero, StartDate, EndDate, Validate, ValidatorNotes,
		HomeToOfficeFixedMiles
	from
		subcats
	where
		(
			subcatid = @subcatid
			or @subcatid is null
		)
		and
		(
			subcat = @subcat
			or @subcat is null
		)
	order by
		subcat

RETURN 0

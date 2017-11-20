
CREATE VIEW [dbo].[savedexpenses]
AS
SELECT     expenseid, bmiles, pmiles, reason, receipt, net, vat, total, subcatid, date, staff, others, companyid, returned, home, refnum, claimid, plitres, blitres, 
					  allowanceamount, currencyid, attendees, tip, countryid, foreignvat, convertedtotal, exchangerate, tempallow, reasonid, normalreceipt, receiptattached, 
					  allowancestartdate, allowanceenddate, carid, allowancededuct, allowanceid, nonights, quantity, directors, amountpayable, hotelid, primaryitem, 
					  norooms, vatnumber, personalguests, remoteworkers, accountcode, pencepermile, basecurrency, globalexchangerate, globalbasecurrency, globaltotal, 
					  itemtype, transactionid, journey_unit, AssignmentNumber, esrAssignID, mileageid, hometooffice_deduction_method
FROM         dbo.savedexpenses_current
UNION ALL
SELECT     expenseid, bmiles, pmiles, reason, receipt, net, vat, total, subcatid, date, staff, others, companyid, returned, home, refnum, claimid, plitres, blitres, 
					  allowanceamount, currencyid, attendees, tip, countryid, foreignvat, convertedtotal, exchangerate, tempallow, reasonid, normalreceipt, receiptattached, 
					  allowancestartdate, allowanceenddate, carid, allowancededuct, allowanceid, nonights, quantity, directors, amountpayable, hotelid, primaryitem, 
					  norooms, vatnumber, personalguests, remoteworkers, accountcode, pencepermile, basecurrency, globalexchangerate, globalbasecurrency, globaltotal, 
					  itemtype, transactionid, journey_unit, AssignmentNumber, esrAssignID, mileageid, hometooffice_deduction_method
FROM         dbo.savedexpenses_previous

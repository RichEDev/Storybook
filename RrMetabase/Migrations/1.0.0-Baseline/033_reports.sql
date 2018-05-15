-- <Migration ID="128883ba-9bd1-431f-b4dd-5d4dd578bec8" />
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Home to Office journey step deductions - fastest route', N'A list of home to office deductions from journey steps in the previous tax year for fastest', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'declare @Startdate datetime declare @Enddate datetime declare @TodayYear int declare @TodayMonth int declare @TodayDay int declare @StartYear int declare @EndYear int Set @Todayyear=Year(GETDATE()) Set @TodayMonth=MONTH(getdate()) set @TodayDay=DAY(getdate()) If @TodayMonth>4 set @StartYear=@TodayYear-1; set @EndYear=@TodayYear If @TodayMonth<4 set @StartYear=@TodayYear-2; set @EndYear=@TodayYear-1 IF @TodayDay<6 set @StartYear=@TodayYear-2; set @EndYear=@TodayYear-1 set @StartYear=@TodayYear-1; set @EndYear=@TodayYear set @Startdate = CAST(CAST(@startYear AS VARCHAR(4)) + RIGHT(''0''+ CAST(''04'' AS VARCHAR(2)), 2) + RIGHT(''0'' + CAST(''06'' AS VARCHAR(2)), 2) AS DATETIME) set @Enddate = CAST(CAST(@EndYear AS VARCHAR(4)) + RIGHT(''0'' + CAST(''04'' AS VARCHAR(2)), 2) + RIGHT(''0'' + CAST(''06'' AS VARCHAR(2)), 2) AS DATETIME) select username as [Username],firstname as [Firstname],Surname as [Surname],numActualMiles as [No Miles Claimed],postcodeAnywhereFastestDistance as  [Recommended Distance], num_miles as [Reimbursable Distance],postcodeAnywhereFastestDistance-num_miles AS [Home to Office deduction], CONVERT(VARCHAR(10), savedexpenses.date, 103) as [Date of Expense],coalesce(addresses.AddressName, '') + '', '' + coalesce(addresses.Line1, '') + '', '' + coalesce(addresses.Postcode, '') [From Address],coalesce(end_address.AddressName, '') + '', '' + coalesce(end_address.Line1, '') + '', '' + coalesce(end_address.Postcode, '') [To Address] from savedexpenses inner join savedexpenses_journey_steps on savedexpenses.expenseid=savedexpenses_journey_steps.expenseid inner join claims on claims.claimid=savedexpenses.claimid inner join employees on employees.employeeid=claims.employeeid inner join addresses on addresses.AddressID=savedexpenses_journey_steps.StartAddressID inner join addresses as end_address on end_address.AddressID=savedexpenses_journey_steps.EndAddressID inner join addressDistances on addressDistances.AddressIDA=savedexpenses_journey_steps.StartAddressID and addressDistances.AddressIDB=savedexpenses_journey_steps.EndAddressID where num_miles>0 and claims.datepaid between @startdate and @enddate and (savedexpenses_journey_steps.StartAddressID in (select AddressID from employeeHomeAddresses where employees.employeeid=employeeHomeAddresses.employeeid and savedexpenses.date > startdate and (enddate is null or enddate >savedexpenses.date)) or savedexpenses_journey_steps.EndAddressID in (select AddressID from employeeHomeAddresses where employees.employeeid=employeeHomeAddresses.employeeid and savedexpenses.date > startdate and (enddate is null or enddate >savedexpenses.date)))', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'24d60b28-7e12-417f-adbd-76919e616ca8', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Item Roles and Expense Items', N'A list of all Item Roles defined to the system and the expense items assoicated with the role.', 1, NULL, 0, 0, 0, 0, 3, CAST(N'2013-09-11T20:02:44.393' AS DateTime), NULL, NULL, NULL, N'Select rolename as [Item Role],item_roles.description as [Item Role Description],subcat as [Expense Item],receiptmaximum as [Maximum Without Receipt],maximum as [Maximum with receipt] from item_roles Left join rolesubcats on item_roles.itemroleid=rolesubcats.roleid left join subcats on rolesubcats.subcatid=subcats.subcatid order by rolename,subcat', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'b95dee99-796d-44aa-96f1-66fc0db0c8f9', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Item Setup', N'A list of all expense items defined to the Expenses system and the options selected on that expense item', 1, NULL, 0, 0, 0, 0, 3, CAST(N'2013-11-27T21:26:21.943' AS DateTime), NULL, NULL, NULL, N'Select	[subcats].[subcat] as [Expense Item],
		[subcats].[shortsubcat] as [Expense Item Abbreviated],
		[Category] as [Expense Category],
		[pdname] as [P11d Category],
		[subcats].[accountcode] as [Account Code],
		[subcats].[alternateaccountcode] as [Alternate Account Code],
		[subcats].[description] as [Description],
		[subcats].[comment] as [Comment displayed on item],
		[Total entered as] = Case  [subcats].[addasnet]
			When 0 then ''Gross''
			When 1 then ''NET''
			End,
		[Item reimbursable] = Case [subcats].[reimbursable]
			When 1 then ''Yes''
			Else ''No''
			End,
--Calculation Type
		[Calculation - Item Type] = Case [subcats].[Calculation]
			When 1 then ''Standard Item''
			When 2 then ''Meal''
			When 3 then ''Mileage Pence Per Mile''
			When 4 then ''Daily Alowance''
			When 5 then ''Fuel Receipt''
			When 6 then ''Mileage Based on Fuel Receipt''
			When 7 then ''Fixed Allowance''
			When 8 then ''Fuel Card Mileage''
			When 9 then ''Item Reimburse''
			Else ''Unknown''
			End,
-- Additional Fields Tab
-- General
		[Show Normal Receipt] = Case [subcats].[receiptapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Show VAT Number] = Case [subcats].[Vatnumberapp]
			When 1 then ''Yes''
			Else ''No''
			End,		
		[Is the VAT Number Mandatory] = Case [subcats].[Vatnumbermand]
			When 1 then ''Yes''
			Else ''No''
			End,		
-- Mileage
		[Show Number of Miles] = Case [subcats].[mileageapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Show Number of Passengers] = Case [subcats].[passengersapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Show Number of Personal Miles] = Case [subcats].[pmilesapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Show Number of Business Miles] = Case [subcats].[bmilesapp]
			When 1 then ''Yes''
			Else ''No''
			End,
-- Meals
		[Show Number of Staff] = Case [subcats].[staffapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Show Number of Others] = Case [subcats].[othersapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Show Number of Directors] = Case [subcats].[nodirectorsapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Show Number of Spouses/Partners] = Case [subcats].[nopersonalguestsapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Show Number of Remote Workers] = Case [subcats].[noremoteworkersapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Show event in home city] = Case [subcats].[eventinhomeapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Show Attendees List] = Case [subcats].[attendeesapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Attendees List is Mandatory] = Case [subcats].[attendeesmand]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Show Tip] = Case [subcats].[tipapp]
			When 1 then ''Yes''
			Else ''No''
			End,
-- Hotels
		[Show Number of Nights] = Case [subcats].[nonightsapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Show Number of Rooms] = Case [subcats].[noroomsapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Show Hotel Name/Rating] = Case [subcats].[hotelapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[The Hotel Name is mandatory] = Case [subcats].[hotelmand]
			When 1 then ''Yes''
			Else ''No''
			End,
-- General Details Fields
		[Reason] = Case [subcats].[reasonapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Other Details] = Case [subcats].[otherdetailsapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[From] = Case [subcats].[fromapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[To] = Case [subcats].[toapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Company] = Case [subcats].[companyapp]
			When 1 then ''Yes''
			Else ''No''
			End,
-- Roles & Limits Tab
		[subcats].[Startdate] as [Expense Item Start Date],
		[subcats].[Enddate] as [Expense Item End Date],
-- Mileage specific fields
 		[carsize] as [Forced Vehicle Journey Rate Category],
		[Increase vehicle journey rate for passengers] = Case [subcats].[passengersapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Enable Relocation Mileage] = Case [subcats].[isRelocationMileage]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Increase vehicle journey rate for heavy bulky equipment] = Case [subcats].[allowHeavyBulkyMileage]
			When 1 then ''Yes''
			Else ''No''
			End,
-- Home to Office Mileage
		[Enable Home To Ofice Mileage] = Case [subcats].[enableHomeToLocationMileage]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Home To Office is always zero] = Case [subcats].[homeToOfficeAlwaysZero]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Home To Base Calculation] = Case [subcats].[homeToLocationType]
			When 0 then ''Not applicable''
			When 1 then ''Enforce the lesser of Home to address & Office to address''
			When 2 then ''Flag if Home to location is greater than office to location''
			When 3 then ''Deduct Home to Office Distance from journey once''
			When 4 then ''Deduct Home to Office distance every time home is visited''
			When 5 then ''Deduct Home to Office Distance if journey starts or finishes at home''
			When 6 then ''Deduct first and/or last Home to Office Distance from Journey''
			When 7 then ''Deduct Full Home to Office trip every time home is visited''
			When 8 then ''Deduct full home to office distance if journey starts/finishes at home''
			Else ''Not applicable''
			End,
		[Split Number of Others] = Case [subcats].[splitentertainment]
			When 1 then ''Yes''
			Else ''No''
			End,
-- Meals
		[Entertainment].[subcat] as [Split Others To],
		[Split Number of Spouses/Partners] = Case [subcats].[splitpersonal]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Personal Guests].[subcat] as [Split Spouses/Partners To],
		[Split Number of Remote Workers] = Case [subcats].[splitremote]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Remote workers].[subcat] as [Split Remote Workers To],
-- Fixed Allowance
		[subcats].[allowanceamount] as [Allowance Amount],
-- Fuel Card Mileage
 		[Reimbursable_Item].[subcat] as [Reimburasble Item]
 	from [subcats]
	Left Join [Categories] on [Subcats].[Categoryid]=[Categories].[Categoryid]
	Left Join [mileage_categories] on [subcats].[mileageCategory]=[mileage_categories].[mileageid]
	Left Join [pdcats] on [subcats].[pdcatid]=[pdcats].[pdcatid]
	Left Join [subcats] as [Entertainment] on [subcats].[entertainmentid]=[Entertainment].[subcatid]
	Left Join [subcats] as [Personal Guests] on [subcats].[entertainmentid]=[Personal Guests].[subcatid]
	Left Join [subcats] as [Remote workers] on [subcats].[entertainmentid]=[Remote workers].[subcatid]
	Left Join [subcats] as [Reimbursable_Item] on [subcats].[reimbursablesubcatid]=[Reimbursable_Item].[subcatid]
	order by [Expense Item]', 0, N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'82a3ca73-eabc-45b7-914e-935a18aa182e', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'O-Locations Missing A Postcode', N'A list of all locations within the system
', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-10-06T12:12:25.380' AS DateTime), NULL, NULL, 0, N'7bdaf84e-a373-4008-83d1-9e18aaa47f8e', N'22a38fd3-37c0-4518-9e2f-c78fd8ea2c27', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'O-Location List', N'A list of all locations within the system
', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:33:32.880' AS DateTime), NULL, NULL, 0, N'7bdaf84e-a373-4008-83d1-9e18aaa47f8e', N'87d2af99-5012-478d-a68d-161c50d07d95', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Flag summary', N'An overview of each flag and how it''s configured.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, NULL, 0, N'3b9be907-1839-459a-8499-24b12e839bbb', N'a3a486b1-be08-48fb-94bf-bfee4ac85c60', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Sign Off Group List', N'A list of the sign off groups within the system and the individual steps that determine the routing of claim or advance approval', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select groupname as ''Signoff Group Name'',signoffs.stage as [Stage],
 ''Sign-off Type'' = case signofftype 
 when 1 then ''Budget Holder'' 
 when 2 then ''Employee''
 when 3 then ''Team''
 when 4 then ''Line Manager'' 
 when 5 then ''Determined by Claimant''
 when 6 then ''Approval Matrix'' 
 when 7 then ''Determined by Claimant from Approval Matrix'' 
 when 8 then ''Cost Code Owner'' 
 when 9 then ''Assignment Supervisor'' 
 when 100 then ''Scan & Attach''
 when 101 then ''Validation''
 end,
 Approver = case signofftype 
 when 6 then (select name from approvalMatrices where approvalMatrixId=relid) 
 when 7 then (select name from approvalMatrices where approvalMatrixId=relid) 
 when 4 then ''Line Manager''
 when 2 then (select username from employees where employeeid=relid) 
 when 3 then (Select teamname from teams where teamid=relid)
 when 1 then (select budgetholder from budgetholders where budgetholderid=relid) end,
   ''Rule'' = case include when 1 then ''Always'' 
   when 2 then ''Only if claim total is greater than'' 
   when 3 then ''Only if an item exceeds allowed amount''
   when 4 then ''Only if claim includes specified cost code'' 
   when 5 then ''Only if claim total is less than'' 
   when 6 then ''Only if claim includes expense item'' 
   when 7 then ''Only if claim includes expense item older than'' end, 
   ''Rule Value'' = case when (signoffs.include = 1) then '''' when (signoffs.include = 2) then convert(nvarchar(50),signoffs.amount) when (signoffs.include = 3) then convert(nvarchar(50),signoffs.amount) when (signoffs.include = 4) then (select costcode from costcodes where costcodeid=includeid) when (signoffs.include = 5) then convert(nvarchar(50),signoffs.amount) when (signoffs.include = 6) then (select subcat from subcats where subcatid=includeid) when (signoffs.include = 7) then convert(nvarchar(50),signoffs.amount) end, ''Involvement'' = case notify when 2 then ''User is to check claim'' when 1 then ''Just notify user of claim'' end from groups inner join signoffs on signoffs.groupid=groups.groupid order by groups.groupid,stage', 0, N'd6ab6ff4-0ec4-4996-8566-458b816adc0d', N'1177642e-2c66-4d55-9da3-fa388828077a', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by expense item (number of items) last tax year', N'The ten expense items who have claimed the highest number of expenses last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:40:05.997' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'89004f6b-746a-49cd-8c11-fe638b71fc17', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Item Setup - Split Items', N'A list of all expense items where the total amount can be split to other expense items (e.g. a Hotel item may split out to Evening meal and Breakfast)', 1, NULL, 0, 0, 0, 0, 3, CAST(N'2013-11-27T21:26:21.963' AS DateTime), NULL, NULL, NULL, N'Select [subcats].[subcat] as [Primary Expense Item],[Split_Subcat].[subcat] as [Split Expense Item] from [subcats]
	inner Join [subcat_split] on [subcats].[subcatid]=[subcat_split].[primarysubcatid]
	inner join [subcats] as [Split_subcat] on [subcat_split].[splitsubcatid]=[Split_subcat].[subcatid]
	Where [subcats].[subcatid] in (Select [Primarysubcatid] from [subcat_split])
	Order by [Primary Expense Item],[Split Expense Item]', 0, N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'8cd5233e-a7a1-4159-b03b-992824cb499e', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Mobile Usage - No claimants and Device Types', N'A list of Mobile device types and the number of claimants that have linked the device type to their Expenses Account', 1, NULL, 0, 0, 0, 0, 3, CAST(N'2014-05-31T06:40:10.860' AS DateTime), NULL, CAST(N'2014-06-02T18:11:56.330' AS DateTime), NULL, N'Select [metabaseExpenses].[dbo].[mobileDeviceTypes].[model],Count(Distinct([mobiledevices].[Employeeid])) as [No of Employees] from [mobileDevices]  Inner Join [metabaseExpenses].[dbo].[mobileDeviceTypes] on Mobiledevices.deviceTypeID=[metabaseExpenses].[dbo].[mobileDeviceTypes].[mobileDeviceTypeID]  inner Join [employees] on [mobileDevices].[employeeID]=[employees].[employeeid]  where [deviceSerialKey]<>'''' and [employees].[archived]=0 group by [deviceTypeID],[metabaseExpenses].[dbo].[mobileDeviceTypes].[model] order by [model]', 0, N'0145e0a2-53fe-4cf7-a310-d63a80d7c72b', N'3eef1912-0faf-42ea-8250-5cb0970eb4f3', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Claim stage', N'I would like to see what stage any claim is at that hasn''t reached the final stage.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2015-12-24T09:34:29.090' AS DateTime), NULL, N'
select 
clm.stage as [Current Stage],
clm.name as [Claim Name],
emp.firstname as [First Name],
emp.surname as [Surname],
clm.total as [Claim Total],
clm.amountpayable as [Claim Amount Payable],
clm.datesubmitted as [Date Submitted],
clm.paid as [Claim Paid?]
from  claims clm 
left join employees emp on emp.employeeid=clm.employeeid
where  clm.stage < (select max(stage) from signoffs as tempsignoffs where groupid = emp.groupid)
and (datesubmitted between GETDATE()-30 AND GETDATE())
order by clm.datesubmitted desc,clm.name desc
', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'8f1662a3-67e3-4065-b7cb-a49c87e3e3fd', N'64859ed0-542c-4d90-b1b5-b8f9a0089fdd', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Mileage Exceeding Recommended Distance (Shortest)', N'A list of claimants mileage items that have exceeded the distance recommended by Postcode Anywhere when using the shortest route', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select DISTINCT
claims.datepaid as [Date Paid], 
firstname as [First Name], 
surname as [Surname], 
username as [User Name], 
claims.name as [Claim Name], 
date as [Date], 
refnum as [Reference Number], 
addressesFrom.addressFrom as [From], 
addressesTo.addressTo as [To], 
num_miles as [Number of Miles], 
postcodeAnywhereShortestDistance as [Shortest Distance], 
num_miles-postcodeAnywhereShortestDistance as Difference, 
exceeded_recommended_mileage_comment as [Exceeded Mileage Comment], 
mileage_categories.comment as [Vehicle Journey Rate Comment] from savedexpenses 
inner join savedexpenses_journey_steps on savedexpenses.expenseid=savedexpenses_journey_steps.expenseid 
inner join savedexpensesFlags on savedexpenses.expenseid = savedexpensesFlags.expenseid 
And savedexpensesFlags.StepNumber=savedexpenses_journey_steps.step_number
inner join 
claims on claims.claimid=savedexpenses.claimid 
inner join 
employees on employees.employeeid=claims.employeeid 
left join 
mileage_categories on savedexpenses.mileageid=mileage_categories.mileageid 
inner join 
addressesFrom on 
addressesFrom.addressFromId=savedexpenses_journey_steps.StartAddressID 
inner join 
addressesTo on 
addressesTo.addressToId=savedexpenses_journey_steps.EndAddressID 
inner join 
addressDistances on 
addressDistances.AddressIDA=savedexpenses_journey_steps.StartAddressID and 
addressDistances.AddressIDB=savedexpenses_journey_steps.EndAddressID 
where <% WHERE_INSERT %> <% AND_INSERT %> savedexpensesflags.flagType=16 
and claims.datepaid>dateadd(month, -3, dateadd(day, 1, dateadd(day, 
-day(convert(date, getdate())), convert(date, getdate())))) 
order by 
claims.name,claims.datepaid desc', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'6447f393-8db4-4a5c-a474-37632aa3cfb8', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Item Spend Summary By Company', N'Summary of spend by Company Name & Expense Item.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-10-06T09:58:49.500' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'68cb1859-89e4-455e-8d15-384cc5d36d4b', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Mileage - Journey List - Previous Month', N'Provides a list of all the individual journeys undertaken by claimants claiming for mileage expenses during the previous calendar month.', 1, NULL, 0, 0, 0, 0, 3, CAST(N'2017-02-11T06:41:45.630' AS DateTime), NULL, NULL, NULL, N'Select savedexpenses.[Date] as [Date of Expense],
	  subcats.subcat as [Expense Item],
	  Reasons.reason as [Reason for Expense],
	  SJS.num_miles as [Miles], 
	  ADDF.AddressName + '', '' + ADDF.Line1 + '', '' + ADDF.Line2 + '', '' + ADDF.City as [From Address],
	  ADDF.PostcodeLookup as [From Post Code], 
	  ADDT.AddressName + '', '' + ADDT.Line1 + '', '' + ADDT.Line2 + '', '' + ADDT.City as [To Address],
	  ADDT.PostcodeLookup as [To Post Code], 
	  Savedexpenses.DatePaid as [Date Approved] 
	  From Savedexpenses 
	  Left Join savedexpenses_journey_steps as SJS on Savedexpenses.expenseid=SJS.expenseid 
	  Inner Join Addresses as ADDF on SJS.StartAddressID=ADDF.AddressID 
	  Inner Join Addresses as ADDT on SJS.EndAddressID=ADDT.AddressID 
	  Left Join subcats on Savedexpenses.subcatid=subcats.subcatid 
	  Left Join reasons on Savedexpenses.reasonid=Reasons.reasonid 
	  Where savedexpenses.Paid=1 
	  and DatePaid>Getdate()-Day(getdate())-DAY(Getdate()-Day(getdate())) 
	  and DatePaid<Getdate()-Day(getdate()) and subcats.mileageapp=1 
	  and subcats.isRelocationMileage=0 order by [Date of Expense]', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'5bb2aabe-147e-4825-8d6c-46f844b6d618', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by cost code (number of items) last tax year', N'The ten cost codes who have claimed the highest number of expenses last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:11:00.157' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'ce6934a8-fe6c-45fa-ad27-3cc36c36568e', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 Frequent To Location last tax year', N'The top 10 most visited To Location last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:04:32.743' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'43435f71-a920-485e-80e1-3ddc25e07589', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by project code (number of items) last tax year', N'The ten project codes who have claimed the highest number of expenses last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:06:19.333' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'b6b55317-0c1a-4c5c-aa41-3ea837e01566', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by employee (total spent) this tax year', N'The ten employees who have claimed the greatest amount of expenses this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T18:31:56.987' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'0566ccd5-50d0-488c-9c4a-4295b0afeaca', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Odometer Reading', N'A list of employees'' odometer readings v the accumulative business mileage claimed', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-24T11:30:45.260' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'8854fe40-c382-4c23-90c0-449d76d36e63', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 Over-claimed Expense Items this tax year', N'The top 10 most frequently overclaimed expense items this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:11:01.413' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'2c57d641-ce74-4c99-8087-4863df52adaa', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Flagged Expense Items', N'A list of employees and expense items that have a policy exception flag', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-21T09:03:44.520' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'4a2e534f-824b-4e05-bbbc-4a228c8b4650', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Currency List', N'A list of all currencies within the system
', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:25:14.283' AS DateTime), NULL, NULL, 0, N'850422ea-ad71-4cef-b6af-227933bf8065', N'a12b9caa-47a7-4355-8f63-5029d03c1ad6', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 0)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'MBNA Credit Card Outstanding Transactions', N'A report detailing claimants that have MBNA credit card items outstanding (awaiting reconciliation), includes both the statement name and the individual transactions.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select statementdate as [Statement Date],employees.firstname as [First Name],employees.surname as [Surname],transaction_date as [Transaction Date],card_statements_base.name as [Statement Name],transaction_amount as [Amount],description as [Description] from card_transactions_mbna inner join card_transactions on card_transactions.transactionid=card_transactions_mbna.transactionid inner join card_statements_base on card_statements_base.statementid=card_transactions.statementid inner join employee_corporate_cards on card_transactions.corporatecardid=employee_corporate_cards.corporatecardid inner join employees on employees.employeeid=employee_corporate_cards.employeeid where <% WHERE_INSERT %> <% AND_INSERT %> card_transactions.transactionid not in (select transactionid from savedexpenses where transactionid is not null) order by surname,firstname,transaction_date', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'd3e1d0bf-3e11-4bf9-8a9c-10d6e97d95ff', N'6a8fdebc-f249-401d-89e4-e5dd69e55d74', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Items Missing a VAT Receipt 17.5%', N'A list of employees and their expense items that are missing a VAT receipt where the standard VAT rate was 17.5%.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-09-28T10:12:49.610' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'22be65fc-b466-4c37-b419-111b8f77096b', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by expense item (number of items) this tax year', N'The ten expense items who have claimed the highest number of expenses this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:38:51.063' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'897af2e4-c8eb-4896-ab47-128f9c6f05df', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Employee ESR Assignment Details', N'A list of active employee accounts, including their assignment numbers, the start and end dates and whether the assignment number is active.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select username as Username,firstname as Firstname,surname as Surname,esr_assignments.AssignmentNumber as [Assignment Number],esr_assignments.EarliestAssignmentStartDate as [Assignment Start Date],esr_assignments.FinalAssignmentEndDate as [Assigment End Date], ''ESR Assigment Active'' = case esr_assignments.Active when 1 then ''Active'' when 0 then ''Inactive'' end, SupervisorFullname as ''Supervisor Name'',SupervisorAssignmentNumber as ''Supervisor Assignment Number'' from employees inner join esr_assignments on esr_assignments.employeeid=employees.employeeid where <% WHERE_INSERT %> <% AND_INSERT %> employees.active=1 and username not like ''%admin%''', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'07c4b772-1bd4-461c-a052-1693c56976e0', N'8ee530dc-d98e-4efb-964d-505563b49621', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Employee Vehicle List', N'A list of employees and their vehicle details, including the vehicle''s status and details of the journey rate category to which it has been assigned.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-10-06T12:23:22.213' AS DateTime), NULL, NULL, 0, N'618db425-f430-4660-9525-ebab444ed754', N'9b2d88eb-5830-4815-80d6-1709e60f9dc4', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 Over-claimed Expense Items last tax year', N'The top 10 most frequently overclaimed expense items last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:13:59.957' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'a6f154bc-b930-4b9c-8190-1a423e4ebaf5', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by employee (number of items) last tax year', N'The ten employees who have claimed the highest number of expenses last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T18:30:00.250' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'692f5e65-e30f-441c-ae66-23028b6add85', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 Employee Frequent Duplicators this tax year', N'The ten employees who frequently duplicate expense items this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:52:54.777' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'69ca3e8f-30a6-45ab-9c20-24ad8d2438c9', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 Frequent To Location this tax year', N'The top 10 most visited To Location this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:02:36.603' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'850f1ebe-f850-4b1c-be21-2759ee765b7f', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Standard Corporate Card Outstanding Transactions', N'A report detailing claimants that have outstanding (awaiting reconciliation), includes both the statement name and the individual transactions.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select statementdate as [Statement Date],firstname as [First Name],surname as [Surname],transaction_date as [Transaction Date],card_statements_base.name as [Claim Name],transaction_amount as [Amount],description as Description from card_transactions inner join card_statements_base on card_statements_base.statementid=card_transactions.statementid inner join employee_corporate_cards on card_transactions.corporatecardid=employee_corporate_cards.corporatecardid inner join employees on employees.employeeid=employee_corporate_cards.employeeid where <% WHERE_INSERT %> <% AND_INSERT %> card_transactions.transactionid not in (select transactionid from savedexpenses where transactionid is not null) order by surname,firstname,transaction_date', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'242e6037-ecd9-467d-bb96-2b5fa9c54153', N'6a8fdebc-f249-401d-89e4-e5dd69e55d74', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Items Missing a VAT Receipt', N'A list of employees and the expense items missing a VAT receipt', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-24T14:36:53.260' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'0b9b1437-7670-498c-b654-2d16b558849d', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by cost code (number of items) this tax year', N'The ten cost codes who have claimed the highest number of expenses this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:09:39.707' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'68565809-e774-49b4-88ae-2faa34072e15', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Item Spend Summary By Department', N'Summary of spend by Department & Expense Item', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-10-06T10:00:32.007' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'982019e3-f335-4f00-a3c0-32e0317c5fd9', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Business Miles Over 300 Miles Per Day', N'Mileage where daily journeys have exceeded 300 miles', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-09-20T09:28:52.700' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'2fc28e69-6baa-4d2b-9ef3-35212bde629d', N'7fb9d805-f85d-42ad-881f-0d3517b23f9c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Item Spend Summary By Expense Item', N'Summary of spend by expense item (highest spend at the top)', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-10-06T11:14:42.503' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'e1600881-5630-44d9-a993-35b9c881172e', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Mobile Usage - Claimants that have created Expense items based on details submitted from Mobile App', N'Report to show how many Claimants have created Expense items based on submissions from the Mobile App during the last 365 days.', 1, NULL, 0, 0, 0, 0, 3, CAST(N'2014-05-31T06:40:10.863' AS DateTime), NULL, CAST(N'2014-06-02T17:56:03.740' AS DateTime), NULL, N'Select Count(Distinct(Employeeid)) as [No of Employees],COUNT(Expenseid) as [Number of Expense Items] from savedexpenses  inner join claims on savedexpenses.claimid=claims.claimid  Where addedAsMobileItem=1 and [Savedexpenses].[createdon]>GETDATE()-365 ', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'94e11a95-fcec-4778-9745-9ba9dc380590', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Mobile Usage - Claimants that have Mobile items awaiting reconciliation', N'Report to show how many claimants have added details through the mobile app, but have not reconciled them into their claim, within the last 365 days', 1, NULL, 0, 0, 0, 0, 3, CAST(N'2014-05-31T06:40:10.867' AS DateTime), NULL, CAST(N'2014-06-02T17:56:03.740' AS DateTime), NULL, N'Select Count(Distinct(mobileExpenseItems.Employeeid)) as [No of Employees],COUNT(mobileid) as [Number of Expense Items] from Mobileexpenseitems  inner join Employees on mobileExpenseItems.employeeid=employees.employeeid  Where  employees.archived=0 and date>GETDATE()-365 ', 0, N'5e4cba58-d747-45b1-b2df-8f7c218fed18', N'18a72a7d-5779-48b3-bd89-07dc89eb4cce', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Items Missing a VAT Receipt 20%', N'A list of employees and their expense items that are missing a VAT receipt where the standard VAT rate was 20%.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-09-28T10:16:12.120' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'c1642d62-462d-4ad0-8f06-786eaadada7b', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Item Spend Summary', N'Summary of spend by expense item (highest spend at the top)', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-24T11:55:15.397' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'dded926d-d7ae-417a-b154-7a26ccf4dabf', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Barclaycard Purchase Card Outstanding Transactions', N'A report detailing claimants that have Barclaycard purchase card items outstanding (awaiting reconciliation), includes both the statement name and the individual transactions.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select statementdate as [Statement Date],employees.firstname as [First Name],employees.surname as [Surname],transaction_date as [Transaction Date],card_statements_base.name as [Statement Name],transaction_amount as [Amount],description as [Description] from card_transactions_rbs_purchase inner join card_transactions on card_transactions.transactionid=card_transactions_rbs_purchase.transactionid inner join card_statements_base on card_statements_base.statementid=card_transactions.statementid inner join employee_corporate_cards on card_transactions.corporatecardid=employee_corporate_cards.corporatecardid inner join employees on employees.employeeid=employee_corporate_cards.employeeid where <% WHERE_INSERT %> <% AND_INSERT %> card_transactions.transactionid not in (select transactionid from savedexpenses where transactionid is not null) order by surname,firstname,transaction_date', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'1441c92e-fd7a-47c1-b7b5-7cadbd55a7db', N'6a8fdebc-f249-401d-89e4-e5dd69e55d74', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Standard Master Drill Down', N'Comprehensive drill down report ', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-24T14:41:10.740' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'db3eea7d-1cfc-4056-a253-7f80d65f2999', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Item List', N'A list of all expense items within the system
', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:31:48.303' AS DateTime), NULL, NULL, 0, N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'c63f89bc-d570-4115-9095-8292809f38c4', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 Employee Policy Breakers last tax year', N'A list of employees exceeding policy limits last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:59:50.020' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'25ed1d3e-73d4-4ae6-9c98-87d3ba731c62', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Items Without Receipt', N'A list of all expense items where the employee has indicated they have a receipt, 
	but have not attached one', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select employees.firstname + '' '' + employees.surname as [Employee Name], username as [Username], claims_base.name as [Claim], subcats.subcat as [Expense Item], savedexpenses.reason as [Other Details], savedexpenses.total as [Total], savedexpenses.amountpayable as [Amount Payable], savedexpenses.date as [Date of Expense], claims_base.datesubmitted as [Date Submitted], claims_base.datepaid as [Date Paid] from savedexpenses inner join claims_base on savedexpenses.claimid=claims_base.claimid inner join employees on claims_base.employeeid=employees.employeeid inner join subcats on savedexpenses.subcatid=subcats.subcatid where receipt=1 and expenseid not in (select expenseid from receipts)', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'808271d7-658e-4f63-916e-8c268ac8f47c', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Mileage - Journey List', N'Provides a list of all the individual journeys undertaken by claimants claiming for mileage expenses.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-10-13T12:20:53.603' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'202ccefa-9339-4481-8c0a-9821e7664aee', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by department (total spent) this tax year', N'The ten departments who have claimed the greatest amount of expenses this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:35:06.407' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'608a3a9a-3f8c-4e1a-858a-d29867e4193e', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by cost code (total spent) last tax year', N'The ten cost codes who have claimed the greatest amount of expenses last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:15:15.660' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'3f03f676-b311-4b18-813a-d2f392001cdb', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Item Spend Summary By Expense Category', N'Summary of spend by Expense Category ', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-10-06T10:14:38.633' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'9b27ab15-9da2-4ab7-996f-d34fb5924204', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Project Code List', N'A list of all project codes within the system
', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:35:02.537' AS DateTime), NULL, NULL, 0, N'e1ef483c-7870-42ce-be54-ecc5c1d5fb34', N'ae911e2f-c8d7-4738-9031-d52f9204b468', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Odometer Readings by Claim', N'Provides odometer readings and mileage for the current and previous month for individual claims, where odometer readings are entered when the claim is submitted.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'SELECT username AS Username
 ,claims.NAME AS [Claim Name]
 ,firstname AS [First Name]
 ,surname AS [Surname]
 ,title AS [Title]
 ,payroll AS [Payroll Number]
 ,registration AS [Car Registration]
 ,oldreading AS [Previous Reading]
 ,newreading AS [Current Reading]
 ,sum(num_miles) AS [Business Miles]
 ,newreading - oldreading - sum(num_miles) AS [Personal Miles]
 ,convert(VARCHAR, datesubmitted, 103) AS [Date Submitted]
 ,convert(VARCHAR, claims.datepaid, 103) AS [Date Paid]
 ,convert(VARCHAR, dateadd(month, - 1, dateadd(day, 1, dateadd(day, - day(convert(DATE, getdate())), convert(DATE, getdate())))), 103) AS [Date From]
 ,convert(VARCHAR, GETDATE(), 103) AS [Run Date]
FROM savedexpenses
INNER JOIN claims ON claims.claimid = savedexpenses.claimid
INNER JOIN employees ON employees.employeeid = claims.employeeid
INNER JOIN cars ON cars.carid = savedexpenses.carid
INNER JOIN odometer_readings ON convert(VARCHAR, odometer_readings.datestamp, 103) = convert(VARCHAR, claims.datesubmitted, 103)
 AND odometer_readings.carid = cars.carid
INNER JOIN savedexpenses_journey_steps ON savedexpenses.expenseid = savedexpenses_journey_steps.expenseid
WHERE <% WHERE_INSERT %> <% AND_INSERT %> claims.datepaid > (
  SELECT min(datestamp)
  FROM odometer_readings
  WHERE datestamp >= dateadd(month, - 1, dateadd(day, 1, dateadd(day, - day(convert(DATE, getdate())), convert(DATE, getdate()))))
  )
 AND fuelcard = 1
GROUP BY username
 ,claims.NAME
 ,cars.carid
 ,firstname
 ,surname
 ,title
 ,payroll
 ,registration
 ,claims.datepaid
 ,savedexpenses.carid
 ,claims.datesubmitted
 ,oldreading
 ,newreading
ORDER BY username
 ,datesubmitted', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'33a40242-6efb-45fa-a229-d8d9344fb438', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'RBS Credit Card Outstanding Transactions', N'A report detailing claimants that have RBS credit card items outstanding (awaiting reconciliation), includes both the statement name and the individual transactions.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select statementdate as [Statement Date],employees.firstname as [First Name],employees.surname as [Surname],transaction_date as [Transaction Date],card_statements_base.name as [Statement Name],transaction_amount as [Amount],description as [Description] from card_transactions_rbs inner join card_transactions on card_transactions.transactionid=card_transactions_rbs.transactionid inner join card_statements_base on card_statements_base.statementid=card_transactions.statementid inner join employee_corporate_cards on card_transactions.corporatecardid=employee_corporate_cards.corporatecardid inner join employees on employees.employeeid=employee_corporate_cards.employeeid where <% WHERE_INSERT %> <% AND_INSERT %> card_transactions.transactionid not in (select transactionid from savedexpenses where transactionid is not null) order by surname,firstname,transaction_date', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'954a6123-dffb-49e1-8445-dbc8aa98b8cb', N'6a8fdebc-f249-401d-89e4-e5dd69e55d74', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Home to Office journey step deductions - shortest', N'A list of home to office deductions from journey steps in the previous tax year for shortest route', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'SELECT claims.datepaid as [Date Paid], firstname as [First Name],  surname as [Surname], username as [User Name],  claims.name as [Claim Name], date as [Date],  refnum as [Reference Number],  addressesFrom.addressFrom as [From],  addressesFromDetails.postcode AS [From Postcode],  addressesTo.addressTo as [To],  addressesToDetails.postcode AS [To postcode],  num_miles as [Number of Miles], postcodeAnywhereShortestDistance as [Shortest Distance],  num_miles-postcodeAnywhereShortestDistance as Difference, exceeded_recommended_mileage_comment as [Exceeded Mileage Comment],  mileage_categories.comment as [Vehicle Journey Rate Comment]  from savedexpenses  inner join savedexpenses_journey_steps on savedexpenses.expenseid=savedexpenses_journey_steps.expenseid  inner join claims on claims.claimid=savedexpenses.claimid  inner join employees on employees.employeeid=claims.employeeid  left join mileage_categories on savedexpenses.mileageid=mileage_categories.mileageid  inner join addressesFrom on addressesFrom.addressFromId=savedexpenses_journey_steps.StartAddressID  inner join addressesTo on addressesTo.addressToId=savedexpenses_journey_steps.EndAddressID  inner join addresses as addressesFromDetails on addressesFromDetails.AddressID=savedexpenses_journey_steps.StartAddressID  inner join addresses as addressesToDetails on addressesToDetails.AddressID=savedexpenses_journey_steps.EndAddressID  inner join addressDistances on addressDistances.AddressIDA=savedexpenses_journey_steps.StartAddressID and addressDistances.AddressIDB=savedexpenses_journey_steps.EndAddressID  where  <% WHERE_INSERT %> <% AND_INSERT %> claims.datepaid > dateadd(month, -1, dateadd(day, 1, dateadd(day, -day(convert(date, getdate())), convert(date, getdate())))) and claims.datepaid<dateadd(month, 0,dateadd(day, 1, dateadd(day, -day(convert(date, getdate())), convert(date, getdate()))))  order by claims.name,claims.datepaid desc', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'379f70f4-afa6-4c69-b625-16901fbcd306', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Items Exceeding Policy Limit', N'A list of employees who have gone over their limit and the number of times that has occurred', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-20T16:53:51.933' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'41b99642-bc82-40f1-b50c-03515c455b62', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'American Express Daily Credit Card Outstanding Transactions', N'A report detailing claimants that have daily American Express credit card items outstanding (awaiting reconciliation), includes both the statement name and the individual transactions.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select statementdate as [Statement Date],employees.firstname as [First Name],employees.surname as [Surname],transaction_date as [Transaction Date],card_statements_base.name as [Statement Name],transaction_amount as [Amount],description as [Description] from card_transactions_amex_daily inner join card_transactions on card_transactions.transactionid=card_transactions_amex_daily.transactionid inner join card_statements_base on card_statements_base.statementid=card_transactions.statementid inner join employee_corporate_cards on card_transactions.corporatecardid=employee_corporate_cards.corporatecardid inner join employees on employees.employeeid=employee_corporate_cards.employeeid where <% WHERE_INSERT %> <% AND_INSERT %> card_transactions.transactionid not in (select transactionid from savedexpenses where transactionid is not null) order by surname,firstname,transaction_date', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'07296d0b-9b68-4651-9578-035c05315032', N'6a8fdebc-f249-401d-89e4-e5dd69e55d74', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Item Spend Summary By Project Code', N'Summary of spend by Project Code & Expense Item', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-10-06T10:01:40.627' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'a791fc4a-7468-40ed-b5a1-042e0c78be4d', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Allstar Outstanding Transactions', N'A report detailing claimants that have any Allstar items outstanding (awaiting reconciliation), includes both the statement name and the individual transactions.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select statementdate as [Statement Date],firstname as [First Name],surname as [Surname],transactionreg as [Vehicle registration],transaction_date as [Transaction Date],card_statements_base.name as [Statement Name],transaction_amount as [Amount],description as Description from card_transactions_allstar inner join card_transactions on card_transactions.transactionid=card_transactions_allstar.transactionid inner join card_statements_base on card_statements_base.statementid=card_transactions.statementid inner join employee_corporate_cards on card_transactions.corporatecardid=employee_corporate_cards.corporatecardid inner join employees on employees.employeeid=employee_corporate_cards.employeeid where <% WHERE_INSERT %> <% AND_INSERT %> card_transactions.transactionid not in (select transactionid from savedexpenses where transactionid is not null) order by card_number', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'e6042ae6-5842-4574-b77e-04b5ca043a20', N'6a8fdebc-f249-401d-89e4-e5dd69e55d74', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by cost code (total spent) this tax year', N'The ten cost codes who have claimed the greatest amount of expenses this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:13:29.863' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'e06e700a-0b04-4f98-a97d-07d03fd0fde3', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Employees Missing a Line Manager ', N'A report detailing active claimants without a Line Manager.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select username as Username,firstname as [First Name],surname as Surname from employees where <% WHERE_INSERT %> <% AND_INSERT %> active=1 and linemanager is null and username not like ''%admin%''', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'65f0d26e-4cff-4862-8f47-08c5b6a91cf2', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Employee Delegates List', N'A list of employees that have set delegates for their login account and who the delagates are', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select employees.username as Username, employees.firstname as Firstname, employees.surname as Surname, delegate.username as [Delegate Username], delegate.firstname as [Delegate Firstname], delegate.surname as [Delegate Surname] from employees inner join employee_proxies on employees.employeeid=employee_proxies.employeeid inner join employees as delegate on delegate.employeeid=employee_proxies.proxyid <% WHERECMD_INSERT %> <% WHERE_INSERT %>', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'01c054a1-025f-455c-9a8c-0df44a2daf69', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 0)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'P11D Category List', N'A list of all p11d categories within the system
', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-18T11:26:54.200' AS DateTime), NULL, NULL, 0, N'5d2d9191-83ea-4ed5-8a46-0aabb8190392', N'885e7906-5a18-48ca-b545-54092a142662', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Personal v Business Miles For Last Month', N'By employee, the number of  business miles and personal miles travelled in the last month.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-09-28T09:27:26.630' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'ece05acd-5690-4eea-bce8-56f6d59f6f47', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Mileage Exceeding Recommended Distance (Fastest)', N'A list of claimants mileage items that have exceeded the distance recommended by Postcode Anywhere when using the fastest route', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select 
claims.datepaid as [Date Paid],
firstname as [First Name],
surname as [Surname], 
username as [User Name], 
claims.name as [Claim Name], 
date as [Date], 
refnum as [Reference Number], 
addressesFrom.addressFrom as [From] , 
addressesTo.addressTo as [To], 
num_miles as [Number of Miles], 
postcodeAnywhereFastestDistance as [Fastest Distance], 
num_miles-postcodeAnywhereFastestDistance as Difference, 
exceeded_recommended_mileage_comment as [Exceeded Mileage Comment], 
mileage_categories.comment as [Vehicle Journey Rate Comment] from savedexpenses 
inner join savedexpenses_journey_steps on savedexpenses.expenseid=savedexpenses_journey_steps.expenseid 
inner join savedexpensesFlags on savedexpenses.expenseid = savedexpensesFlags.expenseid 
And savedexpensesFlags.StepNumber=savedexpenses_journey_steps.step_number
inner join 
claims on claims.claimid=savedexpenses.claimid 
inner join 
employees on employees.employeeid=claims.employeeid 
left join 
mileage_categories on savedexpenses.mileageid=mileage_categories.mileageid 
inner join 
addressesFrom on
addressesFrom.addressFromId=savedexpenses_journey_steps.StartAddressID 
inner join 
addressesTo on addressesTo.addressToId=savedexpenses_journey_steps.EndAddressID 
inner join 
addressDistances on
addressDistances.AddressIDA=savedexpenses_journey_steps.StartAddressID 
and addressDistances.AddressIDB=savedexpenses_journey_steps.EndAddressID 
where <% WHERE_INSERT %> <% AND_INSERT %> savedexpensesflags.flagType=16 
and claims.datepaid>dateadd(month, -3, dateadd(day, 1, dateadd(day, -day(convert(date,getdate())), convert(date, getdate())))) 
order by 
claims.name, claims.datepaid desc', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'511f93ad-3ae1-402b-ad04-58568494891f', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense claims awaiting approval with approver', N'Expense claims that have been submitted, but haven''t yet been approved for payment.', 1, NULL, 0, 0, 0, 0, 3, CAST(N'2012-09-26T20:09:53.633' AS DateTime), NULL, CAST(N'2014-06-02T17:48:15.737' AS DateTime), NULL, N'Select [employees].[username] AS [Username], [employees].[title] AS [Title], [employees].[firstname] AS [Firstname], [employees].[surname] AS [Surname], [employees].[email] AS [Email Address], [Checkandpay].[datesubmitted] AS [Date Submitted], [checkandpay].[name] AS [Claim Name], [claims_base].[description] AS [Claim Description], SUM([savedexpenses].[amountpayable]) AS [Amount Payable], [groups].[groupname] AS [Signoff Group Name], [checkandpay].[stage] AS [Claim Stage], ''Approver'' = case when [claims_base].[teamid] is not null then (select teamname from teams where teamid=[claims_base].[teamid]) when [checkandpay].[checkerid] is not null then (select username from employees where employeeid=[checkandpay].[checkerid]) When [checkandpay].[itemcheckerid] is not null then (select username from employees where employeeid=[checkandpay].[itemcheckerid]) when [costcodes].[Owneremployeeid] is not null then (select username from employees where employeeid=[costcodes].[owneremployeeid]) when [costcodes].[Ownerteamid] is not null then (select Teamname from teams where Teamid=[costcodes].[ownerteamid]) end, ''Approver Title'' = Case when [claims_base].[teamid] is not null then '''' when [checkandpay].[checkerid] is not null then (select Title from employees where employeeid=[checkandpay].[checkerid]) When [checkandpay].[itemcheckerid] is not null then (select Title from employees where employeeid=[checkandpay].[itemcheckerid]) when [costcodes].[Owneremployeeid] is not null then (select Title from employees where employeeid=[costcodes].[owneremployeeid]) when [costcodes].[Ownerteamid] is not null then '''' end, ''Approver Firstname'' = Case when [claims_base].[teamid] is not null then '''' when [checkandpay].[checkerid] is not null then (select Firstname from employees where employeeid=[checkandpay].[checkerid]) When [checkandpay].[itemcheckerid] is not null then (select Firstname from employees where employeeid=[checkandpay].[itemcheckerid]) when [costcodes].[Owneremployeeid] is not null then (select firstname from employees where employeeid=[costcodes].[owneremployeeid]) when [costcodes].[Ownerteamid] is not null then '''' end, ''Approver Surname'' = Case when [claims_base].[teamid] is not null then '''' when [checkandpay].[checkerid] is not null then (select surname from employees where employeeid=[checkandpay].[checkerid]) When [checkandpay].[itemcheckerid] is not null then (select surname from employees where employeeid=[checkandpay].[itemcheckerid]) when [costcodes].[Owneremployeeid] is not null then (select surname from employees where employeeid=[costcodes].[owneremployeeid]) when [costcodes].[Ownerteamid] is not null then '''' end, ''Approver Email address'' = Case when [claims_base].[teamid] is not null then '''' when [checkandpay].[checkerid] is not null then (select email from employees where employeeid=[checkandpay].[checkerid]) When [checkandpay].[itemcheckerid] is not null then (select email from employees where employeeid=[checkandpay].[itemcheckerid]) when [costcodes].[Owneremployeeid] is not null then (select email from employees where employeeid=[costcodes].[owneremployeeid]) when [costcodes].[Ownerteamid] is not null then '''' end from checkandpay Left join Employees on checkandpay.employeeid=employees.employeeid Left join claims_base on checkandpay.claimid=claims_base.claimid Left Join savedexpenses on checkandpay.claimid=savedexpenses.claimid Left join groups on checkandpay.signoffgroup=groups.groupid Left Join savedexpenses_costcodes on savedexpenses.expenseid=savedexpenses_costcodes.expenseid left join costcodes on savedexpenses_costcodes.costcodeid=costcodes.costcodeid <% WHERECMD_INSERT %> <% WHERE_INSERT %> Group by [employees].[username], [employees].[title], [employees].[firstname], [employees].[surname], [employees].[email], [Checkandpay].[datesubmitted], [checkandpay].[name], [claims_base].[description], [groups].[groupname], [checkandpay].[stage], [Checkandpay].[checkerid], [Checkandpay].[itemcheckerid], [claims_base].[teamid], [costcodes].[OwnerEmployeeId], [costcodes].[OwnerTeamId] ', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'6b235f77-cb84-43ff-9da8-5dceb5adf31e', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Barclaycard Credit Card Outstanding Transactions', N'A report detailing claimants that have Barclaycard credit card items outstanding (awaiting reconciliation), includes both the statement name and the individual transactions.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select statementdate as [Statement Date],firstname as [First Name],surname as [Surname],transaction_date as [Transaction Date],card_statements_base.name as [Claim Name],transaction_amount as [Amount],description as Description from card_transactions_barclaycard inner join card_transactions on card_transactions.transactionid=card_transactions_barclaycard.transactionid inner join card_statements_base on card_statements_base.statementid=card_transactions.statementid inner join employee_corporate_cards on card_transactions.corporatecardid=employee_corporate_cards.corporatecardid inner join employees on employees.employeeid=employee_corporate_cards.employeeid where <% WHERE_INSERT %> <% AND_INSERT %> card_transactions.transactionid not in (select transactionid from savedexpenses where transactionid is not null) order by surname,firstname,transaction_date', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'3ad5a28a-6802-4e25-98ac-64f5b8cf81a6', N'6a8fdebc-f249-401d-89e4-e5dd69e55d74', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Employee Corporate Card Details', N'A report detailing employees with the last 4 digits of their corporate card, the provider and signoff details', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select username as [Username], Surname as [Surname], firstname as [Firstname], email as [Email Address], card_providers.cardprovider as [Card Provider],''************''+RIGHT(cardnumber,4) as [Card Number], ''Card Active'' =case employee_corporate_cards.active when 1 then ''Card Active'' when 2 then ''Card Inactive'' end,groups.groupname as [Credit Card Group Name],pc.groupname as [Purchase Card Group Name] from employees inner join employee_corporate_cards on employees.employeeid=employee_corporate_cards.employeeid inner join card_providers on card_providers.cardproviderid=employee_corporate_cards.cardproviderid left join groups on groups.groupid=employees.groupidcc left join groups as pc on pc.groupid=employees.groupidpc<% WHERECMD_INSERT %><% WHERE_INSERT %> order by Surname,Firstname', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'9df2353e-9f39-4c81-8ba9-66ef40dbf87f', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Item Spend Summary By Cost Code', N'Summary of spend by Cost Code & Expense Item', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-10-06T09:59:54.903' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'73b84e45-01dd-4125-baed-6d0d0b55db20', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Items Missing a Normal Receipt', N'A list of employees and the expense items missing a normal receipt.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-20T16:51:19.687' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'9b8bb643-46d9-4def-8e4c-6f2304a47324', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Receipt Tracker Current & Previous Month', N'Displays information for all claims sent for payment last month', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'SELECT claims_base.claimid AS [Unique Reference]
 ,esrnum.AssignmentNumber AS [Assignment Number]
 ,claims_base.NAME AS [Claim Reference]
 ,employees.surname + '' '' + employees.firstname AS [Claimant Name]
 ,'''' as [Service or Department], supervisor.firstname + '' '' + supervisor.surname as [Supervisor]
 ,convert(date, savedexpenses.date,1) as [Date of Expense]
 ,convert(date, claims_base.datesubmitted,1) as [Date Submitted]
 ,convert(date, claims_base.datepaid,1) as [Date Authorised]
 ,claims.total as [Claim Total]
 , subcats.subcat as [Expense Item]
 , savedexpenses.total as [Item Total]
 , savedexpenses.refnum as [Receipt Reference]
 , employees.employeeid as [Reference Number 1]
 , right(savedexpenses.refnum,6) as [Reference Number 2] 
 from claims_base 
 inner join savedexpenses 
 on claims_base.claimid=savedexpenses.claimid 
 left join esr_assignments as esrnum 
 on savedexpenses.esrAssignID=esrnum.esrAssignID 
 inner join employees 
 on claims_base.employeeid=employees.employeeid 
 left join employees as supervisor 
 on employees.linemanager=supervisor.employeeid 
 inner join claims 
 on savedexpenses.claimid=claims.claimid 
 inner join subcats 
 on savedexpenses.subcatid=subcats.subcatid 
 where <% WHERE_INSERT %> <% AND_INSERT %> claims_base.datepaid >= dateadd(month, -1, dateadd(day, 1, dateadd(day, -day(convert(date, getdate())), convert(date, getdate())))) 
 and subcats.receiptapp = 1 and savedexpenses.normalreceipt = 1 order by [Date Authorised] desc', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'aae31d3e-726f-40e4-bff2-71bc025ff0f4', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Employees Missing A Home Address', N'A report detailing active employee accounts that do not have a home address registered', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select username as [Username],firstname as [First Name],surname as [Surname] from employees where <% WHERE_INSERT %> <% AND_INSERT %> employeeid not in (select employeeid from employeeHomeAddresses) and username not like ''%admin%''', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'8d442456-b76c-4c27-b85d-72745b240834', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 0)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'All employee journey steps (Shortest)', N'All journey steps paid in the previous calendar month.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'SELECT claims.datepaid as [Date Paid], firstname as [First Name],  surname as [Surname], username as [User Name],  claims.name as [Claim Name], date as [Date],  refnum as [Reference Number],  addressesFrom.addressFrom as [From],  addressesFromDetails.postcode AS [From Postcode],  addressesTo.addressTo as [To],  addressesToDetails.postcode AS [To postcode],  num_miles as [Number of Miles], postcodeAnywhereShortestDistance as [Shortest Distance],  num_miles-postcodeAnywhereShortestDistance as Difference, exceeded_recommended_mileage_comment as [Exceeded Mileage Comment],  mileage_categories.comment as [Vehicle Journey Rate Comment]  from savedexpenses  inner join savedexpenses_journey_steps on savedexpenses.expenseid=savedexpenses_journey_steps.expenseid  inner join claims on claims.claimid=savedexpenses.claimid  inner join employees on employees.employeeid=claims.employeeid  left join mileage_categories on savedexpenses.mileageid=mileage_categories.mileageid  inner join addressesFrom on addressesFrom.addressFromId=savedexpenses_journey_steps.StartAddressID  inner join addressesTo on addressesTo.addressToId=savedexpenses_journey_steps.EndAddressID  inner join addresses as addressesFromDetails on addressesFromDetails.AddressID=savedexpenses_journey_steps.StartAddressID  inner join addresses as addressesToDetails on addressesToDetails.AddressID=savedexpenses_journey_steps.EndAddressID  inner join addressDistances on addressDistances.AddressIDA=savedexpenses_journey_steps.StartAddressID and addressDistances.AddressIDB=savedexpenses_journey_steps.EndAddressID  where  <% WHERE_INSERT %> <% AND_INSERT %> claims.datepaid > dateadd(month, -1, dateadd(day, 1, dateadd(day, -day(convert(date, getdate())), convert(date, getdate())))) and claims.datepaid<dateadd(month, 0,dateadd(day, 1, dateadd(day, -day(convert(date, getdate())), convert(date, getdate()))))  order by claims.name,claims.datepaid desc', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'9c8d9e88-46a0-446a-99bc-736743d35d50', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 Employee Policy Breakers this tax year', N'A list of employees exceeding policy limits this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:57:54.663' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'322e0005-473d-4023-9b27-73f3d666ef11', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by expense item (total spent) this tax year', N'The ten expense items who have claimed the greatest amount of expenses this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:41:51.057' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'e22ab8f2-6ba9-42ac-b9c0-99b367194a3a', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Employee Line Managers List', N'A list of active employee accounts and the line manager that has been allocated.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select employees.username as [Username], employees.firstname as [First Name], employees.surname as [Surname],linemanager.username as [Line Manager Username],linemanager.firstname as [Line Manager First Name],linemanager.surname as [Line Manager Surname] from employees inner join employees as linemanager on employees.linemanager=linemanager.employeeid', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'47d7fca9-0f01-4d27-b64c-9a390ef11965', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Freedom of Information Act', N'Expense items by selecting an individual and a date range ', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-24T12:02:04.100' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'acbd0dbe-1faf-494f-bf0b-9b667da141c7', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Department List', N'A list of all departments within the system
', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:26:10.367' AS DateTime), NULL, NULL, 0, N'a0f31cb0-16bb-4ace-aaea-69a7189d9599', N'23896451-e262-4083-866d-9bea0408f948', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by department (total spent) last tax year', N'The ten departments who have claimed the greatest amount of expenses last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:36:47.307' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'c1d9997c-4ef0-4329-8002-9c2e1184a4c0', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Employee List', N'A list of all active employees within the database', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-10-05T15:47:00.297' AS DateTime), NULL, NULL, 0, N'618db425-f430-4660-9525-ebab444ed754', N'22247ce4-e922-462b-ac27-9dbd033e41f1', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 0)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by employee (number of items) this tax year', N'The ten employees who have claimed the highest number of expenses this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T18:28:55.207' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'478c2fb5-1f25-4ce7-bc65-a1a898fc2e7c', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Country List', N'A list of all countries within the system', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:24:01.197' AS DateTime), NULL, NULL, 0, N'0dc9ca2b-74c7-4c9b-ad1c-a66ae55f979d', N'155d18d9-a4d1-4c1e-80f3-a2bd4cab7bb3', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 0)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by project code (total spent) this tax year', N'The ten project codes who have claimed the greatest amount of expenses this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T18:37:04.813' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'ba307345-fe62-48a6-8485-a8e04ca5fb34', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Claims Not Submitted', N'Expense claims group by employee not yet submitted', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-24T11:45:10.083' AS DateTime), NULL, NULL, 0, N'0efa50b5-da7b-49c7-a9aa-1017d5f741d0', N'fdef7897-45ee-4027-be4e-aabe3dbbafe9', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 Employee Frequent Duplicators last tax year', N'The ten employees who frequently duplicate expense items last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:54:49.060' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'd92587d6-d0e9-4fd6-83c9-ac068497a87a', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Items Within a Specific Date Range', N'Expense items within a specified date range', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-24T14:10:29.037' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'3c61a0e1-5d75-48c5-a8b9-adf202306c26', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Item Spend Summary By Reason', N'Summary of spend by Reason & Expense Item', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-10-06T10:03:23.577' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'34d68e6e-4a5f-4e69-9a86-b281ff75a136', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Employee Home Address List', N'A list of active employee records and the home addresses linked to their accounts', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select username as [Username], firstname as [First Name], surname as [Surname], addressFrom as [Home Address], employeeHomeAddresses.StartDate as [Home Address Start Date], employeeHomeAddresses.EndDate as [Home Address End Date] from employees  inner join employeeHomeAddresses on employees.employeeid=employeeHomeAddresses.EmployeeId  inner join addressesFrom on addressesFrom.addressFromId=employeeHomeAddresses.AddressId  where  <% WHERE_INSERT %> <% AND_INSERT %>  username not like ''%admin%''', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'6eb853ba-929d-443c-8b6e-b4204355ece9', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 0)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by department (number of items) this tax year', N'The ten departments who have claimed the highest number of expenses this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:17:23.520' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'5409baff-2422-4090-9d77-b46818a38b4e', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 Infrequent To Location this tax year', N'The top 10 least visited To Location this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:06:44.223' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'6e71e5a2-8460-48c0-90d6-b582153e2b01', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'American Express Monthly Credit Card Outstanding Transactions', N'A report detailing claimants that have monthly American Express credit card items outstanding (awaiting reconciliation), includes both the statement name and the individual transactions.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select statementdate as [Statement Date],employees.firstname as [First Name],employees.surname as [Surname],transaction_date as [Transaction Date],card_statements_base.name as [Statement Name],transaction_amount as [Amount],description as [Description] from card_transactions_amex inner join card_transactions on card_transactions.transactionid=card_transactions_amex.transactionid inner join card_statements_base on card_statements_base.statementid=card_transactions.statementid inner join employee_corporate_cards on card_transactions.corporatecardid=employee_corporate_cards.corporatecardid inner join employees on employees.employeeid=employee_corporate_cards.employeeid where <% WHERE_INSERT %> <% AND_INSERT %> card_transactions.transactionid not in (select transactionid from savedexpenses where transactionid is not null) order by surname,firstname,transaction_date', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'eca56021-c3e3-43e8-9f04-b665067a54d7', N'6a8fdebc-f249-401d-89e4-e5dd69e55d74', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Premier Inn Card Outstanding Transactions', N'A report detailing claimants that have Premier Inn card items outstanding (awaiting reconciliation), includes both the statement name and the individual transactions.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select statementdate as [Statement Date],employees.firstname as [First Name],employees.surname as [Surname],transaction_date as [Transaction Date],card_statements_base.name as [Statement Name],transaction_amount as [Amount],description as [Description] from card_transactions_premierinn inner join card_transactions on card_transactions.transactionid=card_transactions_premierinn.transactionid inner join card_statements_base on card_statements_base.statementid=card_transactions.statementid inner join employee_corporate_cards on card_transactions.corporatecardid=employee_corporate_cards.corporatecardid inner join employees on employees.employeeid=employee_corporate_cards.employeeid where <% WHERE_INSERT %> <% AND_INSERT %> card_transactions.transactionid not in (select transactionid from savedexpenses where transactionid is not null) order by surname,firstname,transaction_date', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'ec6d04d9-7718-4c0a-bb65-b792d47a0179', N'6a8fdebc-f249-401d-89e4-e5dd69e55d74', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Reason List', N'A list of all reasons within the system
', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:36:22.013' AS DateTime), NULL, NULL, 0, N'83077e08-fe7d-4c1a-a306-be4327c349c1', N'7d1c008b-5ca9-4adc-a0fb-b882050da641', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by project code (number of items) this tax year', N'The ten project codes who have claimed the highest number of expenses this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:04:15.937' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'76b937e6-21a3-40bb-8572-b8962adc9fcf', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by project code (total spent) last tax year', N'The ten project codes who have claimed the greatest amount of expenses last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T18:38:50.840' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'e5b9ce50-59ec-47f9-99f9-bc91cc2b4cd9', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Access Role List', N'A list of all allowances within the system
', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-18T10:08:18.490' AS DateTime), NULL, NULL, 0, N'12ded231-b220-4acb-a51d-896c52ff8979', N'0e0eeb65-119c-4712-b7f2-be4c66f728e2', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 0)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Cost Code List', N'A list of all cost codes within the system
', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:22:26.267' AS DateTime), NULL, NULL, 0, N'02009e21-aa1d-4e0d-908a-4e9d73ddfbdf', N'cb5f256e-cfaf-4abc-8687-bfb7dabb2c73', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Allowance List', N'A list of all allowances within the system
', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-18T08:38:18.963' AS DateTime), NULL, NULL, 0, N'68a1116c-b8e7-45d9-824b-acfe82c25c54', N'cfd6fe9d-9186-447d-8da1-c26797275e7b', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Claims Awaiting Approval', N'Expense claims that have been submitted, but haven''t yet been approved for payment.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-10-13T15:38:00.593' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'9b108390-c28e-4a1a-8175-c62913a42e3f', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'All employee journey steps (Quickest)', N'All journey steps paid in the previous calendar month', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select claims.datepaid as [Date Paid], firstname as [First Name], surname as [Surname], username as [User Name], claims.name as [Claim Name], date as [Date], refnum as [Reference Number], addressesFrom.addressFrom as [From], addressesFromDetails.Postcode AS [From Postcode], addressesTo.addressTo as [To], addressesToDetails.Postcode AS [To postcode], num_miles as [Number of Miles], postcodeAnywhereFastestDistance as [Fastest Distance], num_miles-postcodeAnywhereFastestDistance as Difference, exceeded_recommended_mileage_comment as [Exceeded Mileage Comment], mileage_categories.comment as [Vehicle Journey Rate Comment] from savedexpenses inner join savedexpenses_journey_steps on savedexpenses.expenseid=savedexpenses_journey_steps.expenseid inner join claims on claims.claimid=savedexpenses.claimid inner join employees on employees.employeeid=claims.employeeid left join mileage_categories on savedexpenses.mileageid=mileage_categories.mileageid  inner join addressesFrom on addressesFrom.addressFromId=savedexpenses_journey_steps.StartAddressID inner join addressesTo on addressesTo.addressToId=savedexpenses_journey_steps.EndAddressID inner join addresses as addressesFromDetails on addressesFromDetails.AddressID=savedexpenses_journey_steps.StartAddressID inner join addresses as addressesToDetails on addressesToDetails.AddressID=savedexpenses_journey_steps.EndAddressID inner join addressDistances on addressDistances.AddressIDA=savedexpenses_journey_steps.StartAddressID and addressDistances.AddressIDB=savedexpenses_journey_steps.EndAddressID where  <% WHERE_INSERT %> <% AND_INSERT %> claims.datepaid >  dateadd(month, -1, dateadd(day, 1, dateadd(day, -day(convert(date, getdate())), convert(date, getdate())))) and claims.datepaid<dateadd(month, 0,dateadd(day, 1, dateadd(day, -day(convert(date, getdate())), convert(date, getdate())))) order by claims.name, claims.datepaid desc', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'f9174500-e377-4dcb-87d5-c7580a25258d', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Item Spend Summary By Employee', N'Summary of spend by Employee & Expense Item', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-10-06T09:55:15.280' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'bc767c77-8113-47b5-a3a9-c8336bd9f2e2', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Employees with Item Roles', N'List of employees with assoicated Item Roles', 1, NULL, 0, 0, 0, 0, 3, CAST(N'2012-09-26T20:12:56.320' AS DateTime), NULL, NULL, NULL, N'Select username as [Username],surname as [Surname],firstname as [Firstname],item_roles.rolename as [Item_Role] from employees left join employee_roles on employees.employeeid=employee_roles.employeeid left join item_roles on employee_roles.itemroleid=item_roles.itemroleid<% WHERECMD_INSERT %> <% WHERE_INSERT %>', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'88d30478-9bb8-4a97-a012-caa87e971060', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Employees Missing A Work Address', N'A report detailing active employee accounts that do not have a work address registered', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select username as [Username],firstname as [First Name],surname as [Surname] from employees where <% WHERE_INSERT %> <% AND_INSERT %> employeeid not in (select employeeid from employeeWorkAddresses) and username not like ''%admin%''', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'2ec4aaa6-929b-4a4c-8d01-cc6e985b42fb', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 0)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Team List', N'A list of all teams within the system
', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:38:04.470' AS DateTime), NULL, NULL, 0, N'fa495951-4d06-49ad-9f85-d67f9eff4a27', N'1dacf876-cbfc-4a4c-8009-cf9abefa90ed', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 0)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Items No Company', N'A list of employees with expense items where no company has been specified', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-21T08:28:35.680' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'fef7dfe2-f070-44eb-9f7e-d00311e8c0c3', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Employee Work Address List', N'A list of active employee records and the work addresses linked to their accounts', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select username as [Username], firstname as [First Name], surname as [Surname], addressFrom as [Work Address], EmployeeWorkAddresses.StartDate as [Work Address Start Date], EmployeeWorkAddresses.EndDate as [Work Address End Date]  from employees  inner join EmployeeWorkAddresses on employees.employeeid=EmployeeWorkAddresses.employeeID  inner join addressesFrom on addressesFrom.addressFromId=EmployeeWorkAddresses.AddressId  where  <% WHERE_INSERT %> <% AND_INSERT %>  username not like ''%admin%''', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'f040e398-bfb5-47f9-b71b-d0ed67e897dc', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 0)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Personal v Business Miles', N'By employee, the number of  business miles and personal miles travelled', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-24T16:44:18.183' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'f6a7549f-0bbc-4c10-b0ad-d167d4cba47d', N'1609e335-4c11-4b0c-9eeb-d7f58da6484d', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by expense item (total spent) last tax year', N'The ten expense items who have claimed the greatest amount of expenses last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:43:09.750' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'8561649d-80ce-449f-bc93-e359cbc061c4', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Document Check Report - Pool Cars', N'A Listing of all active Pool Cars with the ''Duty of Care" expiry Dates', 1, NULL, 0, 0, 0, 0, 3, CAST(N'2013-02-27T20:25:50.080' AS DateTime), NULL, NULL, NULL, N'SElect [registration] as Registration, [make] as [Vehicle Make], [model] as [Vehicle Model], [Vehicle Engine Type] = Case [cartypeid] When 1 then ''Petrol'' When 2 then ''Diesel'' When 3 then ''LPG'' Else ''Not Defined'' End, [enginesize] as [Engine Size], [insurancenumber] as [Insurance Number], [insuranceexpiry] as [Insurance Expiry Date], [taxexpiry] as [Tax Expiry Date], [mottestnumber] as [MOT Test Number], [motexpiry] as [MOT Expiry Date], [Serviceexpiry] as [Service Expiry Date] from cars where employeeid is null and Active=1', 0, N'a184192f-74b6-42f7-8fdb-6dcf04723cef', N'0ac6254a-b498-4938-92af-e3748234175f', N'7fb9d805-f85d-42ad-881f-0d3517b23f9c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 Infrequent To Location last tax year', N'The top 10 least visited To Location last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:08:06.350' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'3947e2a9-c21b-45b3-b114-e68be153722c', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by department (number of items) last tax year', N'The ten departments who have claimed the highest number of expenses last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:18:57.063' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'fe209e90-8e4e-43c1-b0c2-eba33e2065fb', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Budget Holder List', N'A list of all budget holders within the system

', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T20:20:14.037' AS DateTime), NULL, NULL, 0, N'e740e6dc-ec3e-4a19-810b-eac6abb7782f', N'6780564b-d6e4-4025-82d0-ebec3812b367', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'RBS Purchase Card Outstanding Transactions', N'A report detailing claimants that have RBS purchase card items outstanding (awaiting reconciliation), includes both the statement name and the individual transactions.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select statementdate as [Statement Date],firstname as [First Name],surname as [Surname],transaction_date as [Transaction Date],invoice_number as [Invoice Number],card_statements_base.name as [Statement Name],transaction_amount as [Amount],ticketnumber as [Ticket Number],description as [Description] from card_transactions_barclaycard_enhanced inner join card_transactions on card_transactions.transactionid=card_transactions_barclaycard_enhanced.transactionid inner join card_statements_base on card_statements_base.statementid=card_transactions.statementid inner join employee_corporate_cards on card_transactions.corporatecardid=employee_corporate_cards.corporatecardid inner join employees on employees.employeeid=employee_corporate_cards.employeeid where <% WHERE_INSERT %> <% AND_INSERT %> card_transactions.transactionid not in (select transactionid from savedexpenses where transactionid is not null) order by surname,firstname,transaction_date', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'0f36a50a-0e8e-4617-a72c-ed55bf4bc059', N'6a8fdebc-f249-401d-89e4-e5dd69e55d74', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Barclaycard Credit Card Outstanding Transactions 348', N'A report detailing claimants that have Barclaycard credit card 348 items outstanding (awaiting reconciliation), includes both the statement name and the individual transactions.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select statementdate as [Statement Date],firstname as [First Name],surname as [Surname],transaction_date as [Transaction Date],card_statements_base.name as [Claim Name],transaction_amount as [Amount],description as Description from card_transactions_barclaycard348 inner join card_transactions on card_transactions.transactionid=card_transactions_barclaycard348.transactionid inner join card_statements_base on card_statements_base.statementid=card_transactions.statementid inner join employee_corporate_cards on card_transactions.corporatecardid=employee_corporate_cards.corporatecardid inner join employees on employees.employeeid=employee_corporate_cards.employeeid where <% WHERE_INSERT %> <% AND_INSERT %> card_transactions.transactionid not in (select transactionid from savedexpenses where transactionid is not null) order by surname,firstname,transaction_date', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'faff6410-6e50-49c5-8080-ee9ff4aeb69c', N'6a8fdebc-f249-401d-89e4-e5dd69e55d74', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Recovering VAT Against Mileage', N'Monthly report showing the VAT collected against business mileage entered', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-24T11:20:17.413' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'fafef498-0d1e-43cb-a81a-eed2946288d4', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 spend by employee (total spent) last tax year', N'The ten employees who have claimed the greatest amount of expenses last tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T18:33:55.463' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'53233be5-c436-4791-aafa-f030bfb51bdd', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Top 10 Annual Mileage by employee this tax year', N'Top ten highest mileage incurred by employee this tax year', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-17T19:45:25.150' AS DateTime), NULL, NULL, 10, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'3c85fb3e-8ce6-44e9-8666-f1682a1af603', N'e547a43b-b794-4f4f-8152-5fec24f57001', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Employees Without An Active Assignment Number', N'A list of active employee accounts that do not have an active assignment number associated with them.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'select username as Username,firstname as [First Name],surname as Surname from employees where <% WHERE_INSERT %> <% AND_INSERT %> active=1 and (employeeid not in (select employeeid from esr_assignments) or employeeid not in (select employeeid from esr_assignments where esr_assignments.Active=1)) and username not like ''%admin%''', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'b832a546-be27-4a67-a998-f179e2f27353', N'8ee530dc-d98e-4efb-964d-505563b49621', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Odometer Readings', N'A report showing the odometer readings submitted by employees over a date range.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2010-10-05T15:33:02.583' AS DateTime), NULL, NULL, 0, N'618db425-f430-4660-9525-ebab444ed754', N'd6e8c035-4e99-454c-b839-f9e216bbc831', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Items No Reason', N'A list of employees with expense items where no reason has been specified', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, CAST(N'2009-08-21T08:46:48.987' AS DateTime), NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'30a8bd5a-bb94-4ee9-a40f-f9f5aec2be36', N'c07c5627-294f-4685-91f3-e66667f8fd3e', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Item Setup - VAT', N'A list of all expense items defined to the Expenses system and the VAT settings defined against that expense item', 1, NULL, 0, 0, 0, 0, 3, CAST(N'2013-11-27T21:26:21.957' AS DateTime), NULL, NULL, NULL, N'Select	[subcat] as [Expense Item],
		[VAT Applicable] = Case [subcats].[vatapp]
			When 1 then ''Yes''
			Else ''No''
			End,
		[Date Range Type] = Case  [daterangetype]
			When 0 then ''Before''
			When 1 then ''On or After''
			When 2 then ''Between (inclusive)''
			When 3 then ''Any''
			Else ''Not Defined''
			End,
		[datevalue1] as [Date Value1],
		[datevalue2] as [Date Value2],
		[subcat_vat_rates].[vatamount] as [VAT Rate],
		[subcat_vat_rates].[vatpercent] as [Percentage of Total VAT applied to],
		[VAT Receipt Required] = Case [subcat_vat_rates].[vatreceipt]
			When 1 then ''Yes''
			Else ''No''
			End,
		[subcat_vat_rates].[vatlimitwithout] as [Maximum allowed without VAT Receipt],			
		[subcat_vat_rates].[vatlimitwith] as [Maximum allowed with VAT Receipt]
	from [subcats]
	left join [subcat_vat_rates] on [subcats].[subcatid]=[subcat_vat_rates].[subcatid]
	Order by [Expense Item],[datevalue1]', 0, N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'93796a1e-a2f1-4e06-ab44-8d58056c6d2f', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Expense Item Setup - Allowances', N'A list of all the expense items against which Allowances have been defined', 1, NULL, 0, 0, 0, 0, 3, CAST(N'2013-11-27T21:26:21.960' AS DateTime), NULL, NULL, NULL, N'Select	[subcat] as	[Expense Item],
		[allowances].[allowance] as [Allowance Name],
		[Label] as [Currency],
		[allowances].[Description] as [Description],
		[nighthours] as [Night Rate Details - Number of Hours],
		[nightrate] as [Night Rate Details - Rate],
		[allowancebreakdown].[hours] as [Allowance Rates - Number of Hours],
		[allowancebreakdown].[rate] as [Allowance Rates - Rate]
	from [subcats_allowances]
	Left Join [subcats] on [subcats_allowances].[subcatid]=[subcats].[subcatid]
	left join [allowances] on [subcats_allowances].[allowanceid]=[allowances].[allowanceid]
	Left join [currencies] on [allowances].[currencyid]=[currencies].[currencyid]
	Left Join [global_currencies] on [currencies].[globalcurrencyid]=[global_currencies].[globalcurrencyid]
	Left Join [allowancebreakdown] on [allowances].[allowanceid]=[allowancebreakdown].[allowanceid]
	Order by [Expense Item],[Allowance Name],[Allowance Rates - Number of Hours]', 0, N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'697d7fc2-c5db-40b6-aba4-aee76ad52437', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Vehicle Journey Rate Category Setup', N'A list of all Vehicle Journey Rate Catgeories including details of all date ranges, thresholds and amounts to be paid per unit of travel', 1, NULL, 0, 0, 0, 0, 3, CAST(N'2014-05-31T06:40:10.910' AS DateTime), NULL, CAST(N'2014-06-02T17:45:44.233' AS DateTime), NULL, N'exec [GenerateVehicleJourneyRateStandardReport]', 0, N'5a83aeaf-86c8-48fb-aa2b-e7ab05a74a0b', N'92a73d03-997d-4a0e-b095-1662f4a804b3', N'cd8b3375-65d6-41a5-b956-dfa897da687c', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Failed VAT validation but not amended', N'I would like to capture items that have been included on the financial report WHERE items have failed VAT and may need correcting to ensure correct VAT reclamation', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'ba0c70fc-6131-40aa-bffe-61db0c8d8e94', N'64859ed0-542c-4d90-b1b5-b8f9a0089fdd', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'VAT receipt marked as yes but no receipt attached', N'I would like to run a report on potential overclaim of VAT on the GL file WHERE VAT receipt has been marked as yes but there is no receipt.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'6c488b62-49d2-41ad-a844-1fcd6706303a', N'64859ed0-542c-4d90-b1b5-b8f9a0089fdd', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Claims failed business validation', N'I would like to run a report to show all claims that have failed business validation and haven''t moved to the next stage. This would be beneficial for pay before validate customers to know if claimants have potentially been overpaid but are "sitting" on the claim.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'fcc192d4-fa7f-4b75-b56f-1e60a1cd42bf', N'64859ed0-542c-4d90-b1b5-b8f9a0089fdd', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Claims failed VAT validation', N'I would like to run a report to show all claims that have failed VAT validation and the reasons behind it.', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, NULL, 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'1d0a15a7-47fb-46b5-8356-3accf3f172c9', N'64859ed0-542c-4d90-b1b5-b8f9a0089fdd', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'HSBC Credit Card Outstanding Transactions', N'A report detailing claimants that have HSBC credit card items outstanding (awaiting reconciliation), includes both the statement name and the individual transactions.', 1, NULL, 0, 0, 0, 0, 3, CAST(N'2014-11-29T06:48:16.473' AS DateTime), NULL, NULL, NULL, N'select statementdate as [Statement Date],firstname as [First Name],surname as [Surname],transaction_date as [Transaction Date],card_statements_base.name as [Claim Name],transaction_amount as [Amount],description as Description from card_transactions_ref_hsbc inner join card_transactions on card_transactions.transactionid=card_transactions_ref_hsbc.transactionid inner join card_statements_base on card_statements_base.statementid=card_transactions.statementid inner join employee_corporate_cards on card_transactions.corporatecardid=employee_corporate_cards.corporatecardid inner join employees on employees.employeeid=employee_corporate_cards.employeeid where <% WHERE_INSERT %> <% AND_INSERT %> card_transactions.transactionid not in (select transactionid from savedexpenses where transactionid is not null) order by surname,firstname,transaction_date', 0, N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'3a148217-6c13-4f30-9a2b-0cad804c1b60', N'6a8fdebc-f249-401d-89e4-e5dd69e55d74', NULL, 2)
GO
INSERT [dbo].[reports] ([reportname], [description], [curexportnum], [lastexportdate], [footerreport], [readonly], [forclaimants], [allowexport], [exporttype], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [staticReportSQL], [limit], [basetable], [reportid], [folderid], [footerreportid], [module]) VALUES (N'Duty of Care Health Check', N'A list of all documents by user which is missing information since conversion to the updated Duty of Care feature', 1, NULL, 0, 0, 0, 0, 3, NULL, NULL, NULL, NULL, N'
SELECT * FROM (

SELECT licencee.firstname AS [First Name]
	,licencee.surname AS [Last Name]
	,licencee.username AS [Username]
	,''N/A'' as [Vehicle Registration]
	,''Driving Licence'' as [Document Type]
	,CASE 
		WHEN DrivingLicenceView.Expirydate IS NULL
			THEN ''Missing''
		ELSE cast(datepart(dd, DrivingLicenceView.Expirydate) AS VARCHAR(2)) + ''/'' + cast(datepart(mm, DrivingLicenceView.Expirydate) AS VARCHAR(2)) + ''/'' + cast(datepart(YYYY, DrivingLicenceView.Expirydate) AS VARCHAR(4))
		END AS [Expiry Date]
	,CASE 
		WHEN DrivingLicenceView.Reviewedby IS NULL
			THEN ''Missing''
		ELSE DrivingLicenceView.Reviewedby
		END AS [Reviewed By]
	,CASE 
		WHEN DrivingLicenceView.Document IS NULL
			THEN ''Missing''
		ELSE ''Attached''
		END AS Attachment
	,CASE 
		WHEN DrivingLicenceView.LicenceNumber IS NULL
			THEN ''Missing''
		ELSE ''N/A''--DrivingLicenceView.LicenceNumber
		END AS [Additional Data]
FROM DrivingLicenceView
INNER JOIN employees licencee ON licencee.username = DrivingLicenceView.Employee
WHERE CASE 
		WHEN DrivingLicenceView.Expirydate IS NULL
			THEN 1
		ELSE 0
		END = 1
	OR CASE 
		WHEN DrivingLicenceView.Reviewedby IS NULL
			THEN 1
		ELSE 0
		END = 1
	OR CASE 
		WHEN DrivingLicenceView.Document IS NULL
			THEN 1
		ELSE 0
		END = 1
	OR CASE 
		WHEN DrivingLicenceView.LicenceNumber IS NULL
			THEN 1
		ELSE 0
		END = 1
UNION ALL
SELECT  carowner.firstname as [First Name]
	,carowner.surname as [Last Name]
	,carowner.username as [Username]
	,VehicleDocumentView.Vehicle as [Vehicle Registration]
	,VehicleDocumentView.Documenttype as [Document Type]
	,case 
		when VehicleDocumentView.Expirydate is null then ''Missing''
		else cast(datepart(dd,VehicleDocumentView.Expirydate) as varchar(2)) + ''/'' + cast(datepart(mm,VehicleDocumentView.Expirydate) as varchar(2)) + ''/'' + cast(datepart(YYYY,VehicleDocumentView.Expirydate) as varchar(4))
	 end as [Expiry Date]
	,case 
		when VehicleDocumentView.Reviewedby is null then ''Missing''
		else VehicleDocumentView.Reviewedby  
	 end as [Reviewed By]
	 ,case 
		when VehicleDocumentView.Document is null then ''Missing''
		else ''Attached''
	 end as Attachment
	,case 
		when VehicleDocumentView.Documenttype = ''Mot'' and VehicleDocumentView.Testnumber is null then ''Missing''
		when VehicleDocumentView.Documenttype = ''Mot'' and VehicleDocumentView.Testnumber is not null then VehicleDocumentView.Testnumber
		when VehicleDocumentView.Documenttype = ''Insurance'' and VehicleDocumentView.Policynumber is null then ''Missing''
		when VehicleDocumentView.Documenttype = ''Insurance'' and VehicleDocumentView.Policynumber is not null then VehicleDocumentView.Policynumber
		else ''N/A'' end as [Additional Data]
	FROM VehicleDocumentView
	inner join cars on cars.registration = VehicleDocumentView.Vehicle
	inner join employees carowner on carowner.employeeid = cars.employeeid

	WHERE
		case when VehicleDocumentView.Expirydate is null then 1 else 0 end = 1
		or case when VehicleDocumentView.Reviewedby is null then 1 else 0 end = 1
		or case when VehicleDocumentView.Document is null then 1 else 0 end = 1
		or case 
			when VehicleDocumentView.Documenttype = ''Mot'' and VehicleDocumentView.Testnumber is null then 1
			when VehicleDocumentView.Documenttype = ''Insurance'' and VehicleDocumentView.Policynumber is null then 1
			else 0 end = 1
) Combined
ORDER BY [First Name],[Last Name], [Document Type], [Vehicle Registration]

', 0, N'618db425-f430-4660-9525-ebab444ed754', N'c72ad6c1-9221-4414-90cb-1ca54124171f', N'7fb9d805-f85d-42ad-881f-0d3517b23f9c', NULL, 2)
GO

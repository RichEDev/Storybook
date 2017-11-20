CREATE PROCEDURE [dbo].[saveFlagRule]
	@flagID int,
	@flagType TINYINT,
	@action TINYINT,
	@flagText NVARCHAR(MAX),
	@amberTolerance decimal(18,2),
	@frequency TINYINT,
	@frequencyType tinyint,
	@period TINYINT,
	@periodType TINYINT,
	@financialYear int,
	@limit DECIMAL(18,2),
	@dateComparison TINYINT,
	@dateToCompare DATETIME,
	@numberOfMonths TINYINT,
	@description NVARCHAR(MAX),
	@active BIT,
	@date DATETIME,
	@userid int,
	@claimantJustificationRequired bit,
	@displayFlagImmediately bit,
	@noFlagTolerance decimal(18,2),
	@tipLimit decimal(18,2),
	@flagLevel tinyint,
	@approverJustificationRequired bit,
	@increaseByNumOthers bit,
	@displayLimit bit,
	@notesForAuthoriser nvarchar(max),
	@itemRoleInclusionType tinyint,
	@expenseItemInclusionType tinyint,
	@passengerLimit int,
	@employeeID INT,
	@delegateID INT,
	@performItemRoleExpenseCheck bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if @performItemRoleExpenseCheck = 1 and @active = 1 and (select count(flags.flagID) from flags
		left join flagAssociatedRoles on flagAssociatedRoles.flagID = flags.flagID
		left join flagAssociatedExpenseItems on flagAssociatedExpenseItems.flagID = flags.flagID
		where active = 1 and flagtype = @flagType and (@expenseItemInclusionType = 1 or (expenseiteminclusiontype = 1 or subCatID in (select subcatid from flagAssociatedExpenseItems where flagID = @flagID))) and (@itemRoleInclusionType = 1 or (itemroleinclusiontype = 1 or roleID in (select roleID from flagAssociatedRoles where flagID = @flagid))) and flags.flagID <> @flagid) > 0
		begin
			update flags set active = 0 where flagID = @flagID
			return -3	
		end
		
	IF @flagID > 0
		BEGIN
			declare @oldaction TINYINT
			declare @oldflagText NVARCHAR(MAX)
			declare @oldamberTolerance decimal(18,2)
			declare @oldfrequency TINYINT
			declare @oldfrequencyType tinyint
			declare @oldperiod TINYINT
			declare @oldperiodType TINYINT
			declare @oldfinancialYear int
			declare @oldlimit DECIMAL(18,2)
			declare @olddateComparison TINYINT
			declare @olddateToCompare DATETIME
			declare @oldnumberOfMonths TINYINT
			declare @olddescription NVARCHAR(MAX)
			declare @oldactive BIT
			declare @oldclaimantJustificationRequired bit
			declare @olddisplayFlagImmediately bit
			declare @oldnoFlagTolerance decimal(18,2)
			declare @oldtipLimit decimal(18,2)
			declare @oldflagLevel tinyint
			declare @oldapproverJustificationRequired bit
			declare @oldincreaseByNumOthers bit
			declare @olddisplayLimit bit
			declare @oldnotesForAuthoriser nvarchar(max)
			declare @olditemRoleInclusionType tinyint
			declare @oldexpenseItemInclusionType tinyint
			declare @oldpassengerLimit int
			declare @oldLabel nvarchar(max)
			declare @newLabel nvarchar(max)
			
			select @oldaction = action, @oldflagText = flagText, @oldamberTolerance = amberTolerance, @oldfrequency = frequency, @oldfrequencyType = frequencyType, @oldperiod = period, @oldperiodType = periodType, @oldfinancialYear = financialYear, @oldlimit = limit, @olddateComparison = dateComparisonType, @olddateToCompare = dateToCompare, @oldnumberOfMonths = numberOfMonths, @olddescription = description, @oldactive = active, @oldclaimantJustificationRequired = claimantJustificationRequired, @olddisplayFlagImmediately = displayFlagImmediately, @oldnoFlagTolerance = noFlagTolerance, @oldtipLimit = tipLimit, @oldflagLevel = flagLevel, @oldapproverJustificationRequired = approverJustificationRequired, @oldincreaseByNumOthers = increaseByNumOthers, @olddisplayLimit = displayLimit, @oldnotesForAuthoriser = notesForAuthoriser, @olditemRoleInclusionType = itemRoleInclusionType, @oldexpenseItemInclusionType = expenseItemInclusionType, @oldpassengerLimit = passengerLimit from flags where flagID = @flagID
			UPDATE flags SET [action] = @action, flagText = @flagText, amberTolerance = @amberTolerance, frequency = @frequency, frequencyType = @frequencyType, period = @period, periodType = @periodType, financialYear = @financialYear, limit = @limit, dateComparisonType = @dateComparison, dateToCompare = @dateToCompare, numberOfMonths = @numberofmonths, modifiedOn = @date, modifiedBy = @userid, [description] = @description, active = @active, claimantJustificationRequired = @claimantJustificationRequired, displayFlagImmediately = @displayFlagImmediately, noFlagTolerance = @noFlagTolerance, tipLimit = @tipLimit, flagLevel = @flagLevel, approverJustificationRequired = @approverJustificationRequired, increaseByNumOthers = @increaseByNumOthers, displayLimit = @displayLimit, notesForAuthoriser = @notesForAuthoriser, itemRoleInclusionType = @itemRoleInclusionType, expenseItemInclusionType = @expenseItemInclusionType, passengerLimit = @passengerLimit WHERE flagID = @flagID
			
			if @oldaction <> @action
				begin
					if @oldaction = 1
						set @oldLabel = 'Flag Item'
					else if @oldaction = 2
						set @oldLabel = 'Block Item'
					
					if @action = 1
						set @newLabel = 'Flag Item'
					else if @action = 2
						set @newLabel = 'Block Item'
						
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '64B63FD2-D5F1-4555-9CAD-555DB458510E', @oldLabel, @newLabel, @description, null;
				end
				
			if @oldflagText <> @flagText
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '24EC644D-37D4-4DE2-A4D2-51060E210F4D', @oldflagText, @flagText, @description, null;
				
			if @oldamberTolerance <> @amberTolerance
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, 'D8A2A99D-3257-4F62-88AA-F694CC455032', @oldamberTolerance, @amberTolerance, @description, null;
				
			if @oldfrequency <> @frequency
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '70F5E58A-6C80-464D-8103-9388D953AEA3', @oldfrequency, @frequency, @description, null;
				
			if @oldfrequencyType <> @frequencyType
				begin
					if @oldfrequencyType = 1
						set @oldLabel = 'In the last'
					else if @oldfrequencyType = 2
						set @oldLabel = 'Every'
					
					if @frequencyType = 1
						set @newLabel = 'In the last'
					else if @frequencyType = 2
						set @newLabel = 'Every'
						
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, 'AE80B9C7-6DD6-4C0F-BC5F-840CE20E4BBA', @oldLabel, @newLabel, @description, null;
				end
				
			if @oldperiod <> @period
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '0A6A29E2-4A30-432F-BB57-79B5EE81A043', @oldperiod, @period, @description, null;
				
			if @oldperiodType <> @periodType
				begin
					if @oldperiodType = 1
						set @oldLabel = 'Days'
					else if @oldperiodType = 2
						set @oldLabel = 'Weeks'
					else if @oldperiodType = 3
						set @oldLabel = 'Months'
					else if @oldperiodType = 4
						set @oldLabel = 'Years'
					else if @oldperiodType = 5
						set @oldLabel = 'Calendar Weeks'
					else if @oldperiodType = 6
						set @oldLabel = 'Calendar Months'
					else if @oldperiodType = 7
						set @oldLabel = 'Calendar Years'
					else if @oldperiodType = 8
						set @oldLabel = 'Financial Years'
						
					if @periodType = 1
						set @newLabel = 'Days'
					else if @periodType = 2
						set @newLabel = 'Weeks'
					else if @periodType = 3
						set @newLabel = 'Months'
					else if @periodType = 4
						set @newLabel = 'Years'
					else if @periodType = 5
						set @newLabel = 'Calendar Weeks'
					else if @periodType = 6
						set @newLabel = 'Calendar Months'
					else if @periodType = 7
						set @newLabel = 'Calendar Years'
					else if @periodType = 8
						set @newLabel = 'Financial Years'
					
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '910BB181-1C8F-46DF-8375-D5D7370F240F', @oldLabel, @newLabel, @description, null;
				end
				
			if @oldfinancialYear <> @financialYear
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '5E03F3EF-D1BF-4B78-9922-4423779395EC', @oldfinancialYear, @financialYear, @description, null;
				
			if @oldlimit <> @limit
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '808BFD6B-37F9-47A0-B2DE-2EC8375670B9', @oldlimit, @limit, @description, null;
				
			if @olddateComparison <> @dateComparison
				begin
					if @olddateComparison = 1
						set @oldLabel = 'Set Date'
					else if @olddateComparison = 2
						set @oldLabel = 'Last X Months'
						
					if @dateComparison = 1
						set @newLabel = 'Set Date'
					else if @dateComparison = 2
						set @newLabel = 'Last X Months'	
						
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '22CC5C21-7265-4F46-83BF-6DB7FABD2B52', @oldLabel, @newLabel, @description, null;
				end
				
			if @olddateToCompare <> @dateToCompare or (@olddateToCompare is null and @dateToCompare is not null) or (@olddateToCompare is not null and @dateToCompare is null)
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '61E31B86-766A-4202-8DAB-5958C211E5AF', @olddateToCompare, @dateToCompare, @description, null;
				
			if @oldnumberOfMonths <> @numberOfMonths or (@oldnumberOfMonths is null and @numberOfMonths is not null) or (@oldnumberOfMonths is not null and @numberOfMonths is null)
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '6C0A68C2-A2D0-4BED-85E2-801F3CCC9305', @oldnumberOfMonths, @numberOfMonths, @description, null;
				
			if @olddescription <> @description
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '064FEC7E-781A-4B17-A4D7-7CED74CC9CF1', @olddescription, @description, @description, null;
				
			if @oldactive <> @active
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, 'E8082408-620C-465A-9F47-B287037250EA', @oldactive, @active, @description, null;
				
			if @oldclaimantJustificationRequired <> @claimantJustificationRequired
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, 'E361729E-BC3F-4D3C-BA7D-06ED885AD540', @oldclaimantJustificationRequired, @claimantJustificationRequired, @description, null;
				
			if @olddisplayFlagImmediately <> @displayFlagImmediately
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '99FAAD88-0106-4254-AB13-9953D25203F4', @olddisplayFlagImmediately, @displayFlagImmediately, @description, null;
				
			if @oldnoFlagTolerance <> @noFlagTolerance
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '36210C7C-6206-4C0E-908B-6D6E536160ED', @oldnoFlagTolerance, @noFlagTolerance, @description, null;
				
			if @oldtipLimit <> @tipLimit
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '0F74AF40-BE7A-4862-A92E-E69034917C1D', @oldtipLimit, @tipLimit, @description, null;
				
			if @oldflagLevel <> @flagLevel
				begin
					if @oldflagLevel = 1
						set @oldLabel = 'Information only'
					else if @oldflagLevel = 2
						set @oldLabel = 'Amber'
					else if @oldflagLevel = 3
						set @oldLabel = 'Red'
					
					if @flagLevel = 1
						set @newLabel = 'Information only'
					else if @flagLevel = 2
						set @newLabel = 'Amber'
					else if @flagLevel = 3
						set @newLabel = 'Red'
					
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '48278C28-EC77-4207-AC75-2ADA3F586DE6', @oldLabel, @newLabel, @description, null;
				end
				
			if @oldapproverJustificationRequired <> @approverJustificationRequired
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '68C54B15-4B5B-4E4E-91F4-5BAE15EB51ED', @oldapproverJustificationRequired, @approverJustificationRequired, @description, null;
				
			if @oldincreaseByNumOthers <> @increaseByNumOthers
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '977D0BD5-7D7F-436A-A5CC-E1630A750C29', @oldincreaseByNumOthers, @increaseByNumOthers, @description, null;
				
			if @olddisplayLimit <> @displayLimit
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, 'B3EDE559-4A6C-4D7B-97C2-F1D70ED307B6', @olddisplayLimit, @displayLimit, @description, null;
				
			if @oldnotesForAuthoriser <> @notesForAuthoriser
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, 'E41D9A62-31A6-4D6E-8B27-39C70E19F3B6', @oldnotesForAuthoriser, @notesForAuthoriser, @description, null;
				
			if @olditemRoleInclusionType <> @itemRoleInclusionType
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '69F93DDA-E952-4CD0-A43B-8787ADA4B4C3', @olditemRoleInclusionType, @itemRoleInclusionType, @description, null;
				
			if @oldexpenseItemInclusionType <> @expenseItemInclusionType
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, 'C8FD1A3E-34DF-45F9-BAA3-370CD3C6BC92', @oldexpenseItemInclusionType, @expenseItemInclusionType, @description, null;
				
			if @oldpassengerLimit <> @passengerLimit
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 33, @flagID, '0DF3A499-F373-4788-8DDA-91A47CF89C60', @oldpassengerLimit, @passengerLimit, @description, null;
				
		end
	ELSE
		BEGIN
			INSERT INTO flags (flagtype, [action], flagtext, amberTolerance, frequency, frequencyType, period, periodType, financialYear, limit, dateComparisonType, dateToCompare, numberOfMonths, createdOn, createdBy, [description], active, claimantJustificationRequired, displayFlagImmediately, noFlagTolerance, tipLimit, flagLevel, approverJustificationRequired, increaseByNumOthers, displayLimit, notesForAuthoriser, itemRoleInclusionType, expenseItemInclusionType, passengerLimit) VALUES (@flagType, @action, @flagText, @amberTolerance, @frequency, @frequencyType, @period, @periodType, @financialYear, @limit, @dateComparison, @dateToCompare, @numberOfMonths, @date, @userid, @description, @active, @claimantJustificationRequired, @displayFlagImmediately, @noFlagTolerance, @tipLimit, @flagLevel, @approverJustificationRequired, @increaseByNumOthers, @displayLimit, @notesForAuthoriser, @itemRoleInclusionType, @expenseItemInclusionType, @passengerLimit)
			SET @flagID = SCOPE_IDENTITY()
			
			exec addInsertEntryToAuditLog @employeeID, @delegateID, 33, @flagID, @description, null;
		END

	RETURN @flagID
END

GO
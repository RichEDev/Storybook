

CREATE PROCEDURE [dbo].[MergeAddresses] 
	@mergeIDs nvarchar(max),
	@newCompanyID int,
	@userID int,
	@delegateID int
AS
BEGIN TRY
	BEGIN
		BEGIN TRANSACTION MergeAddresses
			DECLARE @ID int
			DECLARE @Value nvarchar(10)
			DECLARE @Count int
			SET @Count = 0;
			DECLARE @fromCount int;
			SET @fromCount = 0;
			DECLARE @toCount int;
			SET @toCount = 0;
			DECLARE @companyCount int;
			SET @companyCount = 0;
			DECLARE @homeCount int;
			SET @homeCount = 0;
			DECLARE @workCount int;
			SET @workCount = 0;
			
			DECLARE @expenseid int;
			DECLARE @step_number int;
			DECLARE @recordTitle nvarchar(2000);
			DECLARE @oldAddress nvarchar(250);
			DECLARE @newAddress nvarchar(250);
			DECLARE @expenseDate datetime;
			DECLARE @claimName nvarchar(50);
			DECLARE @username nvarchar(50);
			DECLARE @employeeLocationID int;
			
			--Set the name of the address being merged to for the audit log
			SET @newAddress = dbo.GetAddressLocationName(@newCompanyID)
			
			DECLARE @tmpTable TABLE (
			fromCount int, toCount int,
			companyCount int, homeCount int,
			workCount int)
			
			DECLARE loop_cursor CURSOR FOR
				SELECT Value FROM dbo.UTILfn_split(@mergeIDs, ',')
				OPEN loop_cursor
				FETCH NEXT FROM loop_cursor INTO @Value
				WHILE @@FETCH_STATUS = 0
				BEGIN
					SET @ID = CAST(@Value AS int);
					SET @Count = 0;
					--Check to see if there are any journey steps to update
					SET @Count = (SELECT count(expenseid) FROM savedexpenses_journey_steps WHERE start_location = @ID OR end_location = @ID);
					IF @Count > 0
					BEGIN
						--Set the name of the address being merged for the audit log
						SET @oldAddress = dbo.GetAddressLocationName(@id);
						
						--Loop all the entries that match the address location ID and update.
						--Add information to the audit log too
						DECLARE journeyStart_cursor CURSOR FOR
							SELECT expenseid, step_number FROM savedexpenses_journey_steps WHERE start_location = @ID
							OPEN journeyStart_cursor
							FETCH NEXT FROM journeyStart_cursor INTO @expenseid, @step_number
							WHILE @@FETCH_STATUS = 0
							BEGIN
								UPDATE savedexpenses_journey_steps SET start_location = @newCompanyID WHERE expenseid = @expenseid AND step_number = @step_number;
								
								
								-- put a direct update to the claim in here
								
								SET @fromCount = @fromCount + 1;
							
								--Set values for the audit log entry
								SET @expenseDate = (SELECT [date] from savedexpenses WHERE expenseid = @expenseid) 
								SET @claimName = (SELECT [name] from claims_base WHERE claimid in (SELECT claimid FROM savedexpenses WHERE expenseid = @expenseid))
								SET @username = (SELECT username from employees WHERE employeeid in (SELECT employeeid from claims_base WHERE claimid in (SELECT claimid FROM savedexpenses WHERE expenseid = @expenseid)))
								SET @recordtitle = (SELECT 'Username: ' + @username + ' Claim: ' + @claimName + ' Mileage expense date: ' + CAST(@expenseDate AS nvarchar(30)) + ' Journey step: ' + CAST(@step_Number AS nvarchar(50)) + ' with expense ID ' + CAST(@expenseid as nvarchar(30)))
								exec addUpdateEntryToAuditLog @userID, @delegateID, 121, 0, '7cd556bf-57f8-45cc-8e20-87ce33a14552', @oldAddress, @newAddress, @recordTitle, null;
								FETCH NEXT FROM journeyStart_cursor INTO @expenseid, @step_number
							END
						CLOSE journeyStart_cursor
						DEALLOCATE journeyStart_cursor
						
						--Loop all the entries that match the address location ID and update.
						--Add information to the audit log too
						DECLARE journeyEnd_cursor CURSOR FOR
							SELECT expenseid, step_number FROM savedexpenses_journey_steps WHERE end_location = @ID
							OPEN journeyEnd_cursor
							FETCH NEXT FROM journeyEnd_cursor INTO @expenseid, @step_number
							WHILE @@FETCH_STATUS = 0
							BEGIN
								UPDATE savedexpenses_journey_steps SET end_location = @newCompanyID WHERE expenseid = @expenseid AND step_number = @step_number;
								SET @toCount = @toCount + 1;
							
								-- put a direct update to the claim in here
							
								--Set values for the audit log entry
								SET @expenseDate = (SELECT [date] from savedexpenses WHERE expenseid = @expenseid) 
								SET @claimName = (SELECT [name] from claims_base WHERE claimid in (SELECT claimid FROM savedexpenses WHERE expenseid = @expenseid))
								SET @username = (SELECT username from employees WHERE employeeid in (SELECT employeeid from claims_base WHERE claimid in (SELECT claimid FROM savedexpenses WHERE expenseid = @expenseid)))
								SET @recordtitle = (SELECT 'Username: ' + @username + ' Claim: ' + @claimName + ' Mileage expense date: ' + CAST(@expenseDate AS nvarchar(30)) + ' Journey step: ' + CAST(@step_Number AS nvarchar(50)) + ' with expense ID ' + CAST(@expenseid as nvarchar(30)))
								exec addUpdateEntryToAuditLog @userID, @delegateID, 121, 0, '3c97fdea-f20f-4650-a10e-6723b68ed596', @oldAddress, @newAddress, @recordTitle, null;
								FETCH NEXT FROM journeyEnd_cursor INTO @expenseid, @step_number
							END
						CLOSE journeyEnd_cursor
						DEALLOCATE journeyEnd_cursor
						
						--Need to recache any associated claims by updating the modified on date
						UPDATE claims_base SET Modifiedon = getutcdate() where claimid in (select claimid from savedexpenses where expenseid in (select expenseid from savedexpenses_journey_steps where start_location = @newCompanyID OR end_location = @newCompanyID))
					END

					--Update company IDs on saved expenses 
					DECLARE curCompany_cursor CURSOR FOR
						SELECT expenseid FROM savedexpenses WHERE companyid = @ID
						OPEN curCompany_cursor
						FETCH NEXT FROM curCompany_cursor INTO @expenseid
						WHILE @@FETCH_STATUS = 0
						BEGIN
							UPDATE savedexpenses SET companyid=@newCompanyID WHERE expenseid = @expenseid;
							SET @companyCount = @companyCount + 1;
							
							--Set values for the audit log entry
							SET @expenseDate = (SELECT [date] from savedexpenses WHERE expenseid = @expenseid) 
							SET @claimName = (SELECT [name] from claims_base WHERE claimid in (SELECT claimid FROM savedexpenses WHERE expenseid = @expenseid))
							SET @username = (SELECT username from employees WHERE employeeid in (SELECT employeeid from claims_base WHERE claimid in (SELECT claimid FROM savedexpenses WHERE expenseid = @expenseid)))
							SET @recordtitle = (SELECT 'Username: ' + @username + ' Claim: ' + @claimName + ' Mileage expense date: ' + CAST(@expenseDate AS nvarchar(30)))
							exec addUpdateEntryToAuditLog @userID, @delegateID, 105, @expenseid, 'f8406705-b459-4067-b838-69f0dbd71da8', @oldAddress, @newAddress, @recordTitle, null;
						FETCH NEXT FROM curCompany_cursor INTO @expenseid
						END
					CLOSE curCompany_cursor
					DEALLOCATE curCompany_cursor

					SET @Count = 0;
					--Update employee home locations
					SET @Count = (SELECT count(employeeLocationID) FROM employeeHomeLocations WHERE locationID = @ID);
					IF @Count > 0
					BEGIN
						DECLARE homeAddress_cursor CURSOR FOR
							SELECT employeeLocationID FROM employeeHomeLocations WHERE locationID = @ID
							OPEN homeAddress_cursor
							FETCH NEXT FROM homeAddress_cursor INTO @employeeLocationID
							WHILE @@FETCH_STATUS = 0
							BEGIN
								UPDATE employeeHomeLocations SET locationID=@newCompanyID WHERE employeeLocationID = @employeeLocationID;
								SET @homeCount = @homeCount + 1;
								SET @username = (SELECT username from employees WHERE employeeid in (SELECT	employeeid FROM employeeHomeLocations WHERE employeeLocationID = @employeeLocationID))
								SET @recordtitle = (SELECT 'Username: ' + @username + ' home location')
								exec addUpdateEntryToAuditLog @userID, @delegateID, 123, @employeeLocationID, 'c3ea2dc0-3971-4e01-b6b9-30727c96bc67', @oldAddress, @newAddress, @recordTitle, null;
							FETCH NEXT FROM homeAddress_cursor INTO @employeeLocationID
							END
						CLOSE homeAddress_cursor
						DEALLOCATE homeAddress_cursor
						--Need to recache the employees so the latest home location is available
						UPDATE employees SET CacheExpiry = getdate() WHERE employeeid in (SELECT employeeid FROM employeeHomeLocations WHERE locationID = @newCompanyID) 
					END
					
					SET @Count = 0;
					--Update employee work locations
					SET @Count = (SELECT count(employeeLocationID) FROM employeeWorkLocations WHERE locationID = @ID);
					IF @Count > 0
					BEGIN
						DECLARE workAddress_cursor CURSOR FOR
							SELECT employeeLocationID FROM employeeWorkLocations WHERE locationID = @ID
							OPEN workAddress_cursor
							FETCH NEXT FROM workAddress_cursor INTO @employeeLocationID
							WHILE @@FETCH_STATUS = 0
							BEGIN
								UPDATE employeeWorkLocations SET locationID=@newCompanyID WHERE employeeLocationID = @employeeLocationID;
								SET @workCount = @workCount + 1;
								SET @username = (SELECT username from employees WHERE employeeid in (SELECT	employeeid FROM employeeWorkLocations WHERE employeeLocationID = @employeeLocationID))
								SET @recordtitle = (SELECT 'Username: ' + @username + ' work location')
								exec addUpdateEntryToAuditLog @userID, @delegateID, 122, @employeeLocationID, '12bba0c5-29f8-45aa-8795-869b5dff65da', @oldAddress, @newAddress, @recordTitle, null;
							FETCH NEXT FROM workAddress_cursor INTO @employeeLocationID
							END
						CLOSE workAddress_cursor
						DEALLOCATE workAddress_cursor
						
						--Need to recache the employees so the latest work location is available
						UPDATE employees SET CacheExpiry = getdate() WHERE employeeid in (SELECT employeeid FROM employeeWorkLocations WHERE locationID = @newCompanyID) 
					END
					
					FETCH NEXT FROM loop_cursor INTO @Value
					
					--Delete the company
					exec deleteCompany @ID, @userID, @delegateID
				END
			CLOSE loop_cursor
			DEALLOCATE loop_cursor

		COMMIT TRANSACTION MergeAddresses
		
		INSERT INTO @tmpTable (fromCount, toCount, companyCount, homeCount, workCount) VALUES (@fromCount, @toCount, @companyCount, @homeCount, @workCount)
		SELECT * FROM @tmpTable
	END
END TRY

BEGIN CATCH
	ROLLBACK TRANSACTION MergeAddresses
	--Raise an error so the on error web service return method can handle the error for the user interface
	raiserror ('An error has occurred in the MergeAddress stored procedure and the merge not committed', 16, 1);
END CATCH

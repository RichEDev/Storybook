CREATE PROCEDURE [dbo].[GetLocationModalAddressLists] 
	-- Add the parameters for the stored procedure here
	@employeeID int,
	@companyType int,
	@listType int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	-- @listType - 0: MostVisited, 1:LastX - SpendManagement.cCompanies.GetRecentAddressType
	-- @companyType - 0: Company, 1: From, 2: To, 3: None - SpendManagement.CompanyType
	
	DECLARE @tmpTable TABLE (
		addressID INT,
		addressName nvarchar(max),
		addressLine1 nvarchar(max),
		city nvarchar(max),
		postcode nvarchar(max),
		isPrivateAddress bit
    )

	IF(@listType = 0)
		BEGIN
		
			DECLARE @SQL NVARCHAR(MAX);
			DECLARE @paramDef nvarchar(110);
			DECLARE @minimumDate DateTime;
			SET @minimumDate = dateadd("M",-6, dbo.GetDatePortion(GetDATE()));
			SET @paramDef = '@empID int, @minimumDate DateTime';
			
			SET @SQL = 'SELECT companyid AS addressID, company AS addressName, address1 AS addressLine1, city AS city, postcode AS postcode, isPrivateAddress AS privateAddress FROM companies WHERE companyid IN ';
		
			IF(@companyType = 0) -- most visited companies
				BEGIN 
					SET @SQL = @SQL + '(SELECT TOP 10 companies.companyid FROM [savedexpenses] INNER JOIN companies ON companies.companyid = savedexpenses.companyid INNER JOIN claims ON claims.claimid = savedexpenses.[claimid] WHERE [savedexpenses].[date] >= @minimumDate AND claims.employeeid = @empID AND companies.archived=0 AND companies.iscompany = 1 GROUP BY companies.companyid ORDER BY COUNT(*) desc)';
				END
			ELSE IF(@companyType = 1) -- most visited from's
				BEGIN 
					SET @SQL = @SQL + '(SELECT TOP 10 start_location FROM [savedexpenses_journey_steps] INNER JOIN [companies] ON [savedexpenses_journey_steps].[start_location] = [companies].[companyid] INNER JOIN savedexpenses ON savedexpenses.expenseid = [savedexpenses_journey_steps].[expenseid] INNER JOIN claims ON claims.claimid = savedexpenses.[claimid] WHERE [savedexpenses].[date] >= @minimumDate AND claims.employeeid = @empID AND companies.archived=0 AND companies.showfrom = 1 GROUP BY start_location ORDER BY COUNT(*) desc)';
				END
			ELSE -- most visited to's
				BEGIN
					SET @SQL = @SQL + '(SELECT TOP 10 end_location FROM [savedexpenses_journey_steps] INNER JOIN [companies] ON [savedexpenses_journey_steps].[end_location] = [companies].[companyid] INNER JOIN savedexpenses ON savedexpenses.expenseid = [savedexpenses_journey_steps].[expenseid] INNER JOIN claims ON claims.claimid = savedexpenses.[claimid] WHERE [savedexpenses].[date] >= @minimumDate AND claims.employeeid = @empID AND companies.archived=0 AND companies.showto = 1 GROUP BY end_location ORDER BY COUNT(*) desc)';
				END
				
			INSERT INTO @tmpTable (addressID, addressName, addressLine1, city, postcode, isPrivateAddress) EXEC sp_executesql @SQL, @paramDef, @empID = @employeeID, @minimumDate = @minimumDate;
		END
	ELSE 
		BEGIN
			--First temporary table 
			declare @table table(companyID int, [date] datetime);
			
			IF(@companyType = 0) -- last visited companies
				BEGIN
					insert into @table(companyID, [date]) SELECT savedexpenses.companyid, savedexpenses.date FROM [savedexpenses] 
					INNER JOIN companies ON companies.companyid = savedexpenses.companyid 
					INNER JOIN claims ON claims.claimid = savedexpenses.[claimid] WHERE claims.employeeid = @employeeID 
					AND companies.iscompany = 1 order by date desc
				END
			ELSE IF(@companyType = 1) -- last visited from's
				BEGIN 
					insert into @table(companyID, [date]) SELECT start_location, savedexpenses.date FROM [savedexpenses_journey_steps] 
					INNER JOIN [companies] ON [savedexpenses_journey_steps].[start_location] = [companies].[companyid] 
					INNER JOIN savedexpenses ON savedexpenses.expenseid = [savedexpenses_journey_steps].[expenseid] 
					INNER JOIN claims ON claims.claimid = savedexpenses.[claimid] WHERE claims.employeeid = @employeeID 
					AND companies.archived=0 AND companies.showfrom = 1 ORDER BY date desc
				END
			ELSE -- last visited to's
				BEGIN
					insert into @table(companyID, [date]) SELECT end_location, savedexpenses.date FROM [savedexpenses_journey_steps] 
					INNER JOIN [companies] ON [savedexpenses_journey_steps].[start_location] = [companies].[companyid] 
					INNER JOIN savedexpenses ON savedexpenses.expenseid = [savedexpenses_journey_steps].[expenseid] 
					INNER JOIN claims ON claims.claimid = savedexpenses.[claimid] WHERE claims.employeeid = @employeeID 
					AND companies.archived=0 AND companies.showfrom = 1 ORDER BY date desc
				END
			
			--Temporary table to store the top 10 unique company IDs
			declare @table2 table(companyID int);
			declare @companyID int;
			declare @count int;
			SET @count = 0;

			DECLARE loop_cursor CURSOR FOR
				SELECT companyID FROM @table
				OPEN loop_cursor
				FETCH NEXT FROM loop_cursor INTO @companyID
				WHILE @@FETCH_STATUS = 0
				BEGIN
					IF NOT EXISTS (SELECT * FROM @table2 WHERE companyID = @companyID)
					BEGIN
						SET @count = @count + 1;
						INSERT INTO @table2 (companyID)VALUES(@companyID)
					END
					
					IF @count = 10
					BEGIN
						BREAK
					END
					ELSE
					BEGIN
						FETCH NEXT FROM loop_cursor INTO @companyID
					END
				END
				
			CLOSE loop_cursor
			DEALLOCATE loop_cursor
			
			INSERT INTO @tmpTable (addressID, addressName, addressLine1, city, postcode, isPrivateAddress) SELECT companies.companyid AS addressID, company AS addressName, address1 AS addressLine1, city AS city, postcode AS postcode, isPrivateAddress AS privateAddress
			FROM companies inner join @table2 t ON t.companyID = companies.companyid 
		END

		SELECT * FROM @tmpTable
END

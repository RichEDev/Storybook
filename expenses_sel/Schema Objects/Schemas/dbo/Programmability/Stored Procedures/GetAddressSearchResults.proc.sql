

CREATE PROCEDURE [dbo].[GetAddressSearchResults] 
	@prefixText nvarchar(250),
	@filterName bit,
	@filterAddress1 bit,
	@filterCity bit,
	@filterCounty bit,
	@filterPostcode bit
	
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX);
	DECLARE @paramDef nvarchar(50);

	SET @paramDef = '@addressText nvarchar(250)';
	
	DECLARE @tmpTable TABLE (
		addressID INT,
		addressName nvarchar(250),
		addressLine1 nvarchar(250),
		city nvarchar(250),
		county nvarchar(250),
		postcode nvarchar(250)
    )
    
    SET @SQL = 'SELECT companyid AS addressID, company AS addressName, address1 AS addressLine1, city AS city, county AS county, postcode AS postcode FROM companies WHERE ';
    
    IF @filterName = 1
		BEGIN
			SET @SQL = @SQL + 'company LIKE @addressText OR ';
		END
	IF @filterAddress1 = 1
		BEGIN
			SET @SQL = @SQL + 'address1 LIKE @addressText OR ';
		END
	IF @filterCity = 1
		BEGIN
			SET @SQL = @SQL + 'city LIKE @addressText OR ';
		END
	IF @filterCounty = 1
		BEGIN
			SET @SQL = @SQL + 'county LIKE @addressText OR ';
		END
	IF @filterPostcode = 1
		BEGIN
			SET @SQL = @SQL + 'postcode LIKE @addressText OR ';
		END
		
		SET @SQL = SUBSTRING(@SQL, 0, len(@SQL) -2);
		
	INSERT INTO @tmpTable (addressID, addressName, addressLine1, city, county, postcode) EXEC sp_executesql @SQL, @paramDef, @addressText = @prefixText
	 SELECT * FROM @tmpTable
END


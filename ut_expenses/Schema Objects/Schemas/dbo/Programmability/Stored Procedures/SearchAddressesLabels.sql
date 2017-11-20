CREATE PROCEDURE [dbo].[SearchAddressesLabels] (@searchTerm NVARCHAR(250))
AS
SET @searchTerm = '%' + @searchTerm + '%';

SELECT al.[AddressLabelID],
	al.[EmployeeID],
	al.[AddressID],
	al.[Label],
	a.[GlobalIdentifier],
	a.[AddressName],
	a.[Line1],
	a.[City],
	a.[Postcode],
	al.[IsPrimary]
FROM [addressLabels] al
INNER JOIN [addresses] a ON al.[AddressID] = a.[AddressID]
WHERE a.[Archived] = 0 AND al.[Label] LIKE @searchTerm
ORDER BY al.[Label];


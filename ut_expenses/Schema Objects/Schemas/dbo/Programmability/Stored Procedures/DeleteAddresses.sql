CREATE PROCEDURE [dbo].[DeleteAddresses]
 @AddressID int,
 @UserID int,
 @DelegateID int
AS
BEGIN
 -- Check for address usage in savedexpenses_journey_steps table
 IF EXISTS (SELECT * FROM [dbo].[savedexpenses_journey_steps] WHERE [StartAddressID] = @AddressID OR [EndAddressID] = @AddressID)
 BEGIN
  RETURN -1;
 END

 -- Check for address usage in ESRAddresses table
 IF EXISTS (SELECT * FROM [dbo].[AddressEsrAllocation] WHERE [AddressID] = @AddressID)
 BEGIN
  RETURN -2;
 END

 -- Check for address usage in employeeHomeLocations table
 IF EXISTS (SELECT * FROM [dbo].[employeeHomeAddresses] WHERE [AddressId] = @AddressID)
 BEGIN
  RETURN -4;
 END

 -- Check for address usage in employeeWorkLocations table
 IF EXISTS (SELECT * FROM [dbo].[EmployeeWorkAddresses] WHERE [AddressId] = @AddressID)
 BEGIN
  RETURN -5;
 END

 -- Check for address usage in organisations and organisationsAddresses tables
 IF EXISTS (SELECT * FROM [dbo].[organisations] WHERE [PrimaryAddressID] = @AddressID) OR EXISTS (SELECT * FROM [dbo].[OrganisationAddresses] WHERE [AddressId] = @AddressID)
 BEGIN
   RETURN -6;
 END

 DECLARE @recordTitle NVARCHAR(MAX) = (SELECT [Line1] + ', ' + [City] + ', ' + [Postcode] FROM [addresses] WHERE [AddressID] = @AddressID);

 DELETE FROM [dbo].[addressDistances] WHERE [AddressIDA] = @AddressID OR [AddressIDB] = @AddressID;
 DELETE FROM [dbo].[addresses] WHERE [AddressID] = @AddressID;

 EXEC addDeleteEntryToAuditLog @UserID, @DelegateID, 179, @AddressID, @recordTitle, NULL;

 RETURN 0;
END
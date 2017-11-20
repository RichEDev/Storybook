CREATE PROCEDURE [dbo].[deleteCompany] 
@companyID int,
@CUemployeeID INT,
@CUdelegateID INT
AS
DECLARE @empHomeCount int;
DECLARE @empWorkCount int;
DECLARE @expStartLocationCount int;
DECLARE @expEndLocationCount int;
DECLARE @company nvarchar(250);
DECLARE @retCode int;

BEGIN
	SET @empHomeCount = (select count(employeeLocationID) from employeeHomeLocations where locationID = @companyID)
	IF @empHomeCount > 0
		RETURN -3;
	SET @empWorkCount = (select count(employeeLocationID) from employeeWorkLocations where locationID = @companyID)
	IF @empWorkCount > 0
		RETURN -4;
	SET @expStartLocationCount = (select count(start_location) from savedexpenses_journey_steps WHERE start_location=@companyid)
	IF @expStartLocationCount > 0
		RETURN -5;
	SET @expEndLocationCount = (select count(end_location) from savedexpenses_journey_steps WHERE end_location=@companyid)
    IF @expEndLocationCount > 0
		RETURN -5;

	declare @tableid uniqueidentifier = (select tableid from tables where tablename = 'companies');
	exec @retCode = dbo.checkReferencedBy @tableid, @companyID
    
	if @retCode = 0
	begin
		SET @company = (SELECT company FROM companies WHERE companyid = @companyID);
    
		DELETE FROM location_distances where locationa = @companyID OR locationb = @companyID
		DELETE FROM companies WHERE companyid = @companyID;
    
		EXEC addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 38, @companyID, @company, null
	end
	RETURN @retCode;
    
END

CREATE PROCEDURE [dbo].[deleteCar]
	@carid INT,
	@userid INT,
	@date datetime,
	@CUemployeeID INT,
	@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @employeeid INT;
	declare @recordTitle nvarchar(2000);
	declare @count int

	--Check if the trust is associated to a financial export
	set @count = (select count(expenseid) from savedexpenses where carid = @carid)
	if @count > 0
		return 1;
	
	declare @tableid uniqueidentifier = (select tableid from tables where tablename = 'cars');
	exec @count = dbo.checkReferencedBy @tableid, @carid;
	if @count <> 0
		return @count;
		
	SELECT @employeeid = employeeid, @recordTitle = registration FROM cars WHERE carid = @carid;

	
	--Check if Duty Of Care record is associated to the car

	
     Declare @vehicleDocumentsTableId varchar(20) =[dbo].[GetEntityId] ('F0247D8E-FAD3-462D-A19D-C9F793F984E8',1)  --c218

	 Declare @attVehicle varchar(20) =[dbo].[GetAttributeId] ('1E5F7E5E-FF90-45E2-A7E5-FF1799668E5F',1)  ---attt2074
	 
	 DECLARE @sqlQuery NVARCHAR(MAX); 

	 set @sqlQuery='

	IF EXISTS (
	SELECT * from '+@vehicleDocumentsTableId+' where '+@attVehicle+' = '+convert (varchar,@carid)+')
		return 2;'

	exec (@sqlQuery)

	delete from cars where carid = @carid;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carid, @recordTitle, null;
   
	IF @employeeid <> 0
		UPDATE employees SET modifiedon = @date, modifiedby = @userid WHERE employeeid = @employeeid;
	
	return 0;
END
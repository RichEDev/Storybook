CREATE PROCEDURE [dbo].[APIdeleteEsrVehicle]
 @ESRVehicleAllocationId bigint 
AS
 DECLARE @carid INT;
 DECLARE @carcount INT;
 SELECT @carid = carid FROM CarAssignmentNumberAllocations WHERE ESRVehicleAllocationId = @ESRVehicleAllocationId;
 SELECT  @carcount = COUNT(carid) FROM CarAssignmentNumberAllocations WHERE CarId = @carid;
 IF	@carcount = 1
 BEGIN
  UPDATE cars SET active = 0 WHERE carid = @carid;
 END

 declare @vjrCount int;
 select
  @vjrCount = count(*)
 from
  ESRVehicles as r
   join CarAssignmentNumberAllocations as rA on rA.ESRVehicleAllocationId = r.ESRVehicleAllocationId
   join ESRVehicles as q on r.UserRatesTable = q.UserRatesTable
        and r.ESRVehicleAllocationId <> q.ESRVehicleAllocationId
    join CarAssignmentNumberAllocations as qA on qA.ESRVehicleAllocationId = q.ESRVehicleAllocationId
              and qA.CarId = rA.CarId
 where
  q.ESRVehicleAllocationId = @ESRVehicleAllocationId;
 
 if @vjrCount = 0
 begin
  declare @mileageid int;
  select
	@mileageid = mileage_categories.mileageid
  from
	ESRVehicles
		join mileage_categories on mileage_categories.UserRatesTable = ESRVehicles.UserRatesTable
  where
	ESRVehicles.ESRVehicleAllocationId = @ESRVehicleAllocationId;

  if @mileageid is not null
    begin
	  DELETE FROM
		[dbo].[car_mileagecats]
	  WHERE
		CarId = @carid
		and mileageid = @mileageid;
	end
 end;

 DELETE FROM [dbo].[CarAssignmentNumberAllocations] WHERE ESRVehicleAllocationId = @ESRVehicleAllocationId;
 DELETE FROM [dbo].[ESRVehicles] WHERE ESRVehicleAllocationId = @ESRVehicleAllocationId;

GO

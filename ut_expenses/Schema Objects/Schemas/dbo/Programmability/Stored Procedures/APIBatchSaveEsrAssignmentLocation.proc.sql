CREATE PROCEDURE [dbo].[APIBatchSaveEsrAssignmentLocation]
 @list ApiBatchSaveEsrAssignmentLocationType READONLY
AS
BEGIN

 DECLARE @ESRAssignmentLocationId int,
   @esrAssignID int,
   @ESRLocationId bigint,
   @StartDate datetime,
   @DeletedDateTime datetime

 declare thecursor cursor fast_forward
 for
 select ESRAssignmentLocationId,
  esrAssignID,
  ESRLocationId,
  StartDate,
  DeletedDateTime
 from @list

 open thecursor;

 fetch next
 from thecursor
 into @ESRAssignmentLocationId,
  @esrAssignID,
  @ESRLocationId,
  @StartDate,
  @DeletedDateTime;

 while @@FETCH_STATUS = 0
 begin

  declare @currentLocation int = null;
  select top 1
   @currentLocation = ESRLocationId
  from
   ESRAssignmentLocations
  where
   esrAssignID = @esrAssignID
   and StartDate <= @StartDate
   and DeletedDateTime is null
  order by
   StartDate desc;

  if @currentLocation is null or @ESRLocationId <> @currentLocation
  begin
   
    -- remove any existing work addresses for this assignment, as an assignment should only have one location
    declare @employeeWorkAddressId int, @employeeId int;
    declare DeleteCursor cursor fast_forward for
 select employeeWorkAddressId, employeeId 
 from employeeworkaddresses
 where ESRAssignmentLocationId in
 (
  select ESRAssignmentLocationId
  from ESRAssignmentLocations
  where esrAssignID = @esrAssignID
 );

 open DeleteCursor;

 fetch next
 from DeleteCursor
 into @employeeWorkAddressId, @employeeId;

 while @@FETCH_STATUS = 0
 begin

  EXEC DeleteEmployeeWorkAddress @employeeWorkAddressId, @employeeId, NULL;

  fetch next
  from DeleteCursor
  into @employeeWorkAddressId, @employeeId;

 end;

 close DeleteCursor;
 deallocate DeleteCursor;

   insert into ESRAssignmentLocations
    values
    (
     @esrAssignID,
     @ESRLocationId,
     @StartDate,
     @DeletedDateTime
    );

   set @ESRAssignmentLocationId = scope_identity();

   insert EmployeeWorkAddresses
    (
     [EmployeeId],
     [AddressId],
     [StartDate],
     [EndDate],
     [Active],
     [Temporary],
     [CreatedOn],
     [CreatedBy],
     [ESRAssignmentLocationId]
    )
    select
     esr_assignments.employeeid,
     AddressEsrAllocation.AddressId,
     @StartDate,
     esr_assignments.EffectiveEndDate,
     1,
     0,
     GETDATE(),
     esr_assignments.CreatedBy,
     @ESRAssignmentLocationId
    from
     esr_assignments
      cross join AddressEsrAllocation
    where
     esr_assignments.esrAssignID = @esrAssignID
     and AddressEsrAllocation.ESRLocationID = @ESRLocationId;

  end
  else
  begin
 declare @effectiveEndDate datetime = null;
 declare @effectiveStartDate datetime = null;
  
 select @effectiveEndDate = EffectiveEndDate, @effectiveStartDate = EffectiveStartDate FROM esr_assignments WHERE esrAssignID = @esrAssignID
  
 update EmployeeWorkAddresses
 set EndDate = @effectiveEndDate
 where ESRAssignmentLocationId IN (SELECT ESRAssignmentLocationId FROM ESRAssignmentLocations WHERE ESRLocationId = @ESRLocationId AND esrAssignID = @esrAssignID)
 and StartDate = @effectiveStartDate
  end
      
  fetch next
  from thecursor
  into @ESRAssignmentLocationId,
   @esrAssignID,
   @ESRLocationId,
   @StartDate,
   @DeletedDateTime;

 end;

 close thecursor;
 deallocate thecursor;

END
GO
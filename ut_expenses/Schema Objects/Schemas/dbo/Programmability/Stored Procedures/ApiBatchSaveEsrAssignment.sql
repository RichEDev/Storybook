CREATE PROCEDURE [dbo].[ApiBatchSaveEsrAssignment] @list ApiBatchSaveEsrAssignmentType READONLY
AS
BEGIN
 DECLARE @index BIGINT
 DECLARE @count BIGINT
 DECLARE @employeeid INT
 DECLARE @ESRPersonId BIGINT
 DECLARE @AssignmentID BIGINT
 DECLARE @EffectiveStartDate DATETIME
 DECLARE @EffectiveEndDate DATETIME
 DECLARE @EarliestAssignmentStartDate DATETIME
 DECLARE @AssignmentType NVARCHAR(1)
 DECLARE @AssignmentNumber NVARCHAR(30)
 DECLARE @SystemAssignmentStatus NVARCHAR(30)
 DECLARE @AssignmentStatus NVARCHAR(80)
 DECLARE @EmployeeStatusFlag NVARCHAR(1)
 DECLARE @PayrollName NVARCHAR(80)
 DECLARE @PayrollPeriodType NVARCHAR(30)
 DECLARE @EsrLocationId BIGINT
 DECLARE @SupervisorFlag NVARCHAR(1)
 DECLARE @SupervisorPersonId BIGINT
 DECLARE @SupervisorAssignmentId BIGINT
 DECLARE @SupervisorAssignmentNumber NVARCHAR(30)
 DECLARE @DepartmentManagerAssignmentId BIGINT
 DECLARE @EmployeeCategory NVARCHAR(2)
 DECLARE @AssignmentCategory NVARCHAR(30)
 DECLARE @PrimaryAssignmentString NVARCHAR(30)
 DECLARE @PrimaryAssignment BIT
 DECLARE @NormalHours FLOAT
 DECLARE @NormalHoursFrequency NVARCHAR(30)
 DECLARE @GradeContractHours FLOAT
 DECLARE @Fte DECIMAL(22, 3)
 DECLARE @FlexibleWorkingPattern NVARCHAR(30)
 DECLARE @EsrOrganisationId BIGINT
 DECLARE @EsrPositionId BIGINT
 DECLARE @PositionName NVARCHAR(240)
 DECLARE @Grade NVARCHAR(240)
 DECLARE @GradeStep NVARCHAR(10)
 DECLARE @StartDateInGrade DATETIME
 DECLARE @AnnualSalaryValue MONEY
 DECLARE @JobName NVARCHAR(120)
 DECLARE @Group NVARCHAR(240)
 DECLARE @TAndAFlag NVARCHAR(240)
 DECLARE @NightWorkerOptOut NVARCHAR(3)
 DECLARE @ProjectedHireDate DATETIME
 DECLARE @VacancyID INT
 DECLARE @ContractEndDate DATETIME
 DECLARE @IncrementDate DATETIME
 DECLARE @MaximumPartTimeFlag NVARCHAR(30)
 DECLARE @AfcFlag NVARCHAR(1)
 DECLARE @EsrLastUpdate DATETIME
 DECLARE @LastWorkingDay DATETIME
 DECLARE @eKSFSpinalPoint NVARCHAR(30)
 DECLARE @ManagerFlag NVARCHAR(30)
 DECLARE @FinalAssignmentEndDate DATETIME
 DECLARE @Active BIT
 DECLARE @esrAssignID INT
 DECLARE @SupervisorEsrAssignId INT
 DECLARE @SignOffOwner VARCHAR(39)
 DECLARE @tmp TABLE (
  tmpID BIGINT
  ,ESRPersonId BIGINT
  ,AssignmentNumber NVARCHAR(30)
  ,EffectiveStartDate DATETIME
  )
 DECLARE @retIds TABLE (
  esrAssignID INT
  ,ESRPersonId BIGINT
  ,AssignmentNumber NVARCHAR(30)
  ,EffectiveStartDate DATETIME
  );

 INSERT @tmp
 SELECT ROW_NUMBER() OVER (
   ORDER BY ESRPersonId
    ,AssignmentNumber
    ,EffectiveStartDate
   )
  ,ESRPersonId
  ,AssignmentNumber
  ,EffectiveStartDate
 FROM @list

 SELECT @count = count(*)
 FROM @tmp

 SET @index = 1

 WHILE @index <= @count
 BEGIN
  SELECT TOP 1 @ESRPersonId = ESRPersonId
   ,@AssignmentNumber = AssignmentNumber
   ,@EffectiveStartDate = EffectiveStartDate
  FROM @tmp
  WHERE tmpID = @index;

  SELECT TOP 1 @employeeid = employeeid
   ,@AssignmentID = AssignmentID
   ,@EffectiveEndDate = EffectiveEndDate
   ,@EarliestAssignmentStartDate = EarliestAssignmentStartDate
   ,@AssignmentType = AssignmentType
   ,@SystemAssignmentStatus = SystemAssignmentStatus
   ,@AssignmentStatus = AssignmentStatus
   ,@EmployeeStatusFlag = EmployeeStatusFlag
   ,@PayrollName = PayrollName
   ,@PayrollPeriodType = PayrollPeriodType
   ,@EsrLocationId = EsrLocationId
   ,@SupervisorFlag = SupervisorFlag
   ,@SupervisorPersonId = SupervisorPersonId
   ,@SupervisorAssignmentId = SupervisorAssignmentId
   ,@SupervisorAssignmentNumber = SupervisorAssignmentNumber
   ,@DepartmentManagerAssignmentId = DepartmentManagerAssignmentId
   ,@EmployeeCategory = EmployeeCategory
   ,@AssignmentCategory = AssignmentCategory
   ,@PrimaryAssignmentString = PrimaryAssignmentString
   ,@PrimaryAssignment = PrimaryAssignment
   ,@NormalHours = NormalHours
   ,@NormalHoursFrequency = NormalHoursFrequency
   ,@GradeContractHours = GradeContractHours
   ,@Fte = Fte
   ,@FlexibleWorkingPattern = FlexibleWorkingPattern
   ,@EsrOrganisationId = EsrOrganisationId
   ,@EsrPositionId = EsrPositionId
   ,@PositionName = PositionName
   ,@Grade = Grade
   ,@GradeStep = GradeStep
   ,@StartDateInGrade = StartDateInGrade
   ,@AnnualSalaryValue = AnnualSalaryValue
   ,@JobName = JobName
   ,@Group = [Group]
   ,@TAndAFlag = TAndAFlag
   ,@NightWorkerOptOut = NightWorkerOptOut
   ,@ProjectedHireDate = ProjectedHireDate
   ,@VacancyID = VacancyID
   ,@ContractEndDate = ContractEndDate
   ,@IncrementDate = IncrementDate
   ,@MaximumPartTimeFlag = MaximumPartTimeFlag
   ,@AfcFlag = AfcFlag
   ,@EsrLastUpdate = EsrLastUpdate
   ,@LastWorkingDay = LastWorkingDay
   ,@eKSFSpinalPoint = eKSFSpinalPoint
   ,@ManagerFlag = ManagerFlag
   ,@FinalAssignmentEndDate = FinalAssignmentEndDate
   ,@Active = Active
   ,@esrAssignID = esrAssignID
   ,@SupervisorEsrAssignId = SupervisorEsrAssignId
   ,@SignOffOwner = SignOffOwner
  FROM @list
  WHERE AssignmentNumber = @AssignmentNumber
   AND ESRPersonId = @ESRPersonId
   AND EffectiveStartDate = @EffectiveStartDate

	IF dbo.CheckAccountProperties('ESRManualSupervisorAssignment',1) = 1
	BEGIN

		SELECT top 1 @SignOffOwner = SignOffOwner
		FROM [esr_assignments]
		WHERE employeeid = @employeeid AND AssignmentNumber = @AssignmentNumber AND (
		EffectiveEndDate < @EffectiveStartDate 
		OR EffectiveStartDate = @EffectiveStartDate
		)
		ORDER BY EffectiveEndDate DESC

	END
	ELSE


  IF EXISTS (
    SELECT esrassignID
    FROM esr_assignments
    WHERE AssignmentNumber = @AssignmentNumber
     AND employeeid = @employeeid
     AND EffectiveStartDate = @EffectiveStartDate
    )
  BEGIN
   SELECT @esrAssignID = esrassignID
   FROM esr_assignments
   WHERE AssignmentNumber = @AssignmentNumber
    AND employeeid = @employeeid
    AND EffectiveStartDate = @EffectiveStartDate
  END

  IF @esrAssignID IS NULL
   OR @esrAssignID = 0
  BEGIN
  
   declare @BeforeMinStartDate datetime = null
     
   select
    @BeforeMinStartDate = MIN(EffectiveStartDate)
   from
    esr_assignments
   where
    AssignmentNumber = @AssignmentNumber
  
   INSERT INTO [dbo].[esr_assignments] (
    [employeeid]
    ,[ESRPersonId]
    ,[AssignmentID]
    ,[EffectiveStartDate]
    ,[EffectiveEndDate]
    ,[EarliestAssignmentStartDate]
    ,[AssignmentType]
    ,[AssignmentNumber]
    ,[SystemAssignmentStatus]
    ,[AssignmentStatus]
    ,[EmployeeStatusFlag]
    ,[PayrollName]
    ,[PayrollPeriodType]
    ,[ESRLocationId]
    ,[SupervisorFlag]
    ,[SupervisorPersonId]
    ,[SupervisorAssignmentId]
    ,[SupervisorAssignmentNumber]
    ,[DepartmentManagerAssignmentId]
    ,[EmployeeCategory]
    ,[AssignmentCategory]
    ,[PrimaryAssignmentString]
    ,[PrimaryAssignment]
    ,[NormalHours]
    ,[NormalHoursFrequency]
    ,[GradeContractHours]
    ,[FTE]
    ,[FlexibleWorkingPattern]
    ,[ESROrganisationId]
    ,[ESRPositionId]
    ,[PositionName]
    ,[Grade]
    ,[GradeStep]
    ,[StartDateInGrade]
    ,[AnnualSalaryValue]
    ,[JobName]
    ,[Group]
    ,[TAndAFlag]
    ,[NightWorkerOptOut]
    ,[ProjectedHireDate]
    ,[VacancyID]
    ,[ContractEndDate]
    ,[IncrementDate]
    ,[MaximumPartTimeFlag]
    ,[AFCFlag]
    ,[ESRLastUpdate]
    ,[LastWorkingDay]
    ,[eKSFSpinalPoint]
    ,[ManagerFlag]
    ,[FinalAssignmentEndDate]
    ,[createdon]
    ,[Active]
    ,[SupervisorEsrAssignID]
    ,[SignOffOwner]
    )
   VALUES (
    @employeeid
    ,@ESRPersonId
    ,@AssignmentID
    ,@EffectiveStartDate
    ,@EffectiveEndDate
    ,@EarliestAssignmentStartDate
    ,@AssignmentType
    ,@AssignmentNumber
    ,@SystemAssignmentStatus
    ,@AssignmentStatus
    ,@EmployeeStatusFlag
    ,@PayrollName
    ,@PayrollPeriodType
    ,@EsrLocationId
    ,@SupervisorFlag
    ,@SupervisorPersonId
    ,@SupervisorAssignmentId
    ,@SupervisorAssignmentNumber
    ,@DepartmentManagerAssignmentId
    ,@EmployeeCategory
    ,@AssignmentCategory
    ,@PrimaryAssignmentString
    ,@PrimaryAssignment
    ,@NormalHours
    ,@NormalHoursFrequency
    ,@GradeContractHours
    ,@Fte
    ,@FlexibleWorkingPattern
    ,@EsrOrganisationId
    ,@EsrPositionId
    ,@PositionName
    ,@Grade
    ,@GradeStep
    ,@StartDateInGrade
    ,@AnnualSalaryValue
    ,@JobName
    ,@Group
    ,@TAndAFlag
    ,@NightWorkerOptOut
    ,@ProjectedHireDate
    ,@VacancyID
    ,@ContractEndDate
    ,@IncrementDate
    ,@MaximumPartTimeFlag
    ,@AfcFlag
    ,@EsrLastUpdate
    ,@LastWorkingDay
    ,@eKSFSpinalPoint
    ,@ManagerFlag
    ,@FinalAssignmentEndDate
    ,GETDATE()
    ,@Active
    ,@SupervisorEsrAssignId
    ,@SignOffOwner
    )

   SET @esrAssignID = scope_identity();

   INSERT INTO @retIds (esrAssignID, ESRPersonId, AssignmentNumber ,EffectiveStartDate)
   VALUES (@esrAssignID, @ESRPersonId, @AssignmentNumber, @EffectiveStartDate);
   
   if @Active = 1 and @EffectiveEndDate is null and @EffectiveStartDate < @BeforeMinStartDate
   begin
    
    update
     esr_assignments
    set
     Active = 0
    where
     AssignmentNumber = @AssignmentNumber
     and EffectiveStartDate > @EffectiveStartDate    
    
   end
   
  END
  ELSE
  BEGIN
   UPDATE [dbo].[esr_assignments]
   SET [employeeid] = @employeeid
    ,[ESRPersonId] = @ESRPersonId
    ,[AssignmentID] = @AssignmentID
    ,[EffectiveStartDate] = @EffectiveStartDate
    ,[EffectiveEndDate] = @EffectiveEndDate
    ,[EarliestAssignmentStartDate] = @EarliestAssignmentStartDate
    ,[AssignmentType] = @AssignmentType
    ,[AssignmentNumber] = @AssignmentNumber
    ,[SystemAssignmentStatus] = @SystemAssignmentStatus
    ,[AssignmentStatus] = @AssignmentStatus
    ,[EmployeeStatusFlag] = @EmployeeStatusFlag
    ,[PayrollName] = @PayrollName
    ,[PayrollPeriodType] = @PayrollPeriodType
    ,[ESRLocationId] = @ESRLocationId
    ,[SupervisorFlag] = @SupervisorFlag
    ,[SupervisorPersonId] = @SupervisorPersonId
    ,[SupervisorAssignmentId] = @SupervisorAssignmentId
    ,[SupervisorAssignmentNumber] = @SupervisorAssignmentNumber
    ,[DepartmentManagerAssignmentId] = @DepartmentManagerAssignmentId
    ,[EmployeeCategory] = @EmployeeCategory
    ,[AssignmentCategory] = @AssignmentCategory
    ,[PrimaryAssignment] = @PrimaryAssignment
    ,[PrimaryAssignmentString] = @PrimaryAssignmentString
    ,[NormalHours] = @NormalHours
    ,[NormalHoursFrequency] = @NormalHoursFrequency
    ,[GradeContractHours] = @GradeContractHours
    ,[FTE] = @Fte
    ,[FlexibleWorkingPattern] = @FlexibleWorkingPattern
    ,[ESROrganisationId] = @EsrOrganisationId
    ,[ESRPositionId] = @EsrPositionId
    ,[PositionName] = @PositionName
    ,[Grade] = @Grade
    ,[GradeStep] = @GradeStep
    ,[StartDateInGrade] = @StartDateInGrade
    ,[AnnualSalaryValue] = @AnnualSalaryValue
    ,[JobName] = @JobName
    ,[Group] = @Group
    ,[TAndAFlag] = @TAndAFlag
    ,[NightWorkerOptOut] = @NightWorkerOptOut
    ,[ProjectedHireDate] = @ProjectedHireDate
    ,[VacancyID] = @VacancyID
    ,[ContractEndDate] = @ContractEndDate
    ,[IncrementDate] = @IncrementDate
    ,[MaximumPartTimeFlag] = @MaximumPartTimeFlag
    ,[AFCFlag] = @AfcFlag
    ,[ESRLastUpdate] = @EsrLastUpdate
    ,[LastWorkingDay] = @LastWorkingDay
    ,[eKSFSpinalPoint] = @eKSFSpinalPoint
    ,[ManagerFlag] = @ManagerFlag
    ,[FinalAssignmentEndDate] = @FinalAssignmentEndDate
    ,[modifiedon] = GETDATE()
    ,[Active] = @Active
    ,[SupervisorEsrAssignId] = @SupervisorEsrAssignId
    ,[SignOffOwner] = @SignOffOwner
   WHERE esrAssignID = @esrAssignID
  END

  SET @index = @index + 1
 END

 SELECT *
 FROM @retIds;

 RETURN 0;
END
GO
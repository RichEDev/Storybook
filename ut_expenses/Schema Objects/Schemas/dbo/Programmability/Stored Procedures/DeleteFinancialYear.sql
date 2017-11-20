CREATE PROCEDURE [dbo].[DeleteFinancialYear] @EmployeeId      INT,
                                            @DelegateId      INT,
                                            @SubAccountId    INT,
                                            @FinancialYearId INT
AS
    IF EXISTS (SELECT mileageid
               FROM   mileage_categories
               WHERE  FinancialYearID = @FinancialYearId)
      BEGIN
          RETURN -1;
      END

    IF EXISTS (SELECT [description]
               FROM   FinancialYears
               WHERE  [Primary] = 1
                      AND FinancialYearID = @FinancialYearId)
      BEGIN
          RETURN -2;
      END

	IF EXISTS (select flagID from flags where financialYear = @FinancialYearId)
		BEGIN
			return -4;
		END
		
    DECLARE @Description NVARCHAR(50)

    SELECT @Description = [description]
    FROM   FinancialYears

    DELETE FROM FinancialYears
    WHERE  FinancialYearID = @FinancialYearId;

    EXEC addDeleteEntryToAuditLog
      @EmployeeId,
      @DelegateID,
      184,
      @FinancialYearID,
      @Description,
      @subAccountId;

    RETURN 0


GO
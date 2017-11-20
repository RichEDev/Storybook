




-- =============================================

-- Author:        <Author,,Name>

-- Create date: <Create Date, ,>

-- Description:   <Description, ,>

-- =============================================

CREATE FUNCTION [dbo].[VehicleJourneyRateIsValid]

(

@vjRate int

)

RETURNS bit

AS

BEGIN

      DECLARE @minDate datetime

      DECLARE @maxDate datetime

      DECLARE @rangeValid bit

      DECLARE @mileageid int

      DECLARE @daterangetype smallint

      DECLARE @datevalue1 datetime

      DECLARE @datevalue2 datetime

      DECLARE @count int

      DECLARE @rangeToValidate int

      DECLARE @rangeCount int

 

      SET @rangeCount = 0;

      SET @rangeValid = 1;

      SET @count = (select count([mileagedateid]) from mileage_dateranges where mileageid = @vjRate and daterangetype <> 3); -- no. ranges not of type Any

      

      IF @count > 0

      BEGIN 

            -- Check Date Ranges for Vehicle Journey Rates

            SET @minDate = convert(datetime, '1785-01-01',120);

            SET @maxDate = convert(datetime, '3000-01-01',120);

            SET @rangeToValidate = DATEDIFF(d, @minDate, @maxDate);

 

            DECLARE loop_cursor CURSOR FOR

            SELECT mileageid, daterangetype, datevalue1, datevalue2 FROM mileage_dateranges WHERE mileageid = @vjRate ORDER BY daterangetype, datevalue1

            OPEN loop_cursor

            FETCH NEXT FROM loop_cursor INTO @mileageid, @daterangetype, @datevalue1, @datevalue2

            WHILE @@FETCH_STATUS = 0

            BEGIN

                  IF @daterangetype = 0 -- Before

                  BEGIN

                        SET @minDate = @datevalue1;

                        SET @rangeToValidate = DATEDIFF(d, @minDate, @maxDate); -- have to minus 1 because max date should be after then val2 for between

                        

                        IF @minDate >= @maxDate

                        BEGIN

                              SET @rangeValid = 0;

                              CLOSE loop_cursor;

                              DEALLOCATE loop_cursor;

 

                              GOTO fnExit;

                        END

                  END

 

                  IF @daterangetype = 1 -- After or Equal To

                  BEGIN

                        SET @maxDate = @datevalue1;

                        SET @rangeToValidate = DATEDIFF(d, @minDate, @maxDate); -- have to minus 1 because max date should be after then val2 for between

                        

                        IF @maxDate < @minDate

                        BEGIN

                              SET @rangeValid = 0;

                              CLOSE loop_cursor;

                              DEALLOCATE loop_cursor;

 

                              GOTO fnExit;

                        END

                  END

 

                  IF @daterangetype = 2 -- Between

                  BEGIN

                        IF @datevalue1 < @minDate OR @datevalue1 >= @maxDate OR @datevalue2 >= @maxDate --OR @datevalue2 < @minDate

                        BEGIN

                              SET @rangeValid = 0;

                              CLOSE loop_cursor;

                              DEALLOCATE loop_cursor;

 

                              GOTO fnExit;

                        END

 

                        DECLARE @tmpCount int;

                        SET @tmpCount = (DATEDIFF(d, @datevalue1, @datevalue2) + 1);

                        SET @rangeCount = (@rangeCount + @tmpCount);

                        

                        IF @rangeCount > @rangeToValidate

                        BEGIN

                              SET @rangeValid = 0;

                              CLOSE loop_cursor;

                              DEALLOCATE loop_cursor;

 

                              GOTO fnExit;

                        END

                  END 

 

                  FETCH NEXT FROM loop_cursor INTO @mileageid, @daterangetype, @datevalue1, @datevalue2

            END

            CLOSE loop_cursor

            DEALLOCATE loop_cursor

 

            IF @rangeCount <> @rangeToValidate AND @minDate <> @maxDate

            BEGIN

                  SET @rangeValid = 0;

                  GOTO fnExit;

            END

      END

      ELSE

      BEGIN

            SET @count = (select count([mileagedateid]) from mileage_dateranges where mileageid = @vjRate and daterangetype = 3); -- no. ranges not of type Any

            IF @count = 0

            BEGIN

                  SET @rangeValid = 0;

                  GOTO fnExit;

            END

      END

 

      DECLARE @minVal DECIMAL(18,2);

      DECLARE @maxVal DECIMAL(18,2);

      DECLARE @valCount DECIMAL(18,2);

      DECLARE @valToValidate DECIMAL(18,2);

      DECLARE @mdId int

      DECLARE @rangetype smallint

      DECLARE @rangevalue1 DECIMAL(18,2)

      DECLARE @rangevalue2 DECIMAL(18,2)

 

      DECLARE loop_cursor2 CURSOR FOR

      SELECT DISTINCT mileagedateid from mileage_dateranges where mileageid = @vjRate

      OPEN loop_cursor2

      FETCH NEXT FROM loop_cursor2 INTO @mdId

      WHILE @@FETCH_STATUS = 0

      BEGIN

            SET @minVal = 0.0;

            SET @maxVal = 999999999.0;

            SET @valCount = 0;

            SET @valToValidate = @maxVal;

 

            SET @count = (select count([mileagethresholdid]) from mileage_thresholds where mileagedateid = @mdId and rangetype <> 3)

 

            IF @count > 0

            BEGIN

                  DECLARE threshold_cursor CURSOR FOR

                  SELECT rangetype, rangevalue1, rangevalue2 FROM mileage_thresholds WHERE mileagedateid = @mdId AND rangetype in (0,2)

                  OPEN threshold_cursor

                  FETCH NEXT FROM threshold_cursor INTO @rangetype, @rangevalue1, @rangevalue2

                  WHILE @@FETCH_STATUS = 0

                  BEGIN

                        IF @rangetype = 0 -- Greater than or equal

                        BEGIN

                              IF @rangevalue1 <= @maxVal

                              BEGIN

                                    SET @maxVal = @rangevalue1;

                                    SET @valToValidate = (@maxVal - @minVal);

                              END

 

                              IF @maxVal < @minVal

                              BEGIN

                                    CLOSE threshold_cursor

                                    DEALLOCATE threshold_cursor

 

                                    CLOSE loop_cursor2

                                    DEALLOCATE loop_cursor2

 

                                    SET @rangeValid = 0;

                                    GOTO fnExit;

                              END

                        END

 

                        IF @rangetype = 2 -- Less than

                        BEGIN

                              IF @rangevalue1 > @minVal

                              BEGIN

                                    SET @minVal = @rangevalue1;

                                    SET @valToValidate = (@maxVal - @minVal);

                              END

 

                              IF @minVal > @maxVal

                              BEGIN

                                    CLOSE threshold_cursor

                                    DEALLOCATE threshold_cursor

 

                                    CLOSE loop_cursor2

                                    DEALLOCATE loop_cursor2

 

                                    SET @rangeValid = 0;

                                    GOTO fnExit;

                              END

                        END

 

                        FETCH NEXT FROM threshold_cursor INTO @rangetype, @rangevalue1, @rangevalue2

                  END

 

                  CLOSE threshold_cursor

                  DEALLOCATE threshold_cursor

 

                  DECLARE threshold_cursor CURSOR FOR

                  SELECT rangetype, rangevalue1, rangevalue2 FROM mileage_thresholds WHERE mileagedateid = @mdId AND rangetype = 1

                  OPEN threshold_cursor

                  FETCH NEXT FROM threshold_cursor INTO @rangetype, @rangevalue1, @rangevalue2

                  WHILE @@FETCH_STATUS = 0

                  BEGIN

                        IF @rangetype = 1 -- Between

                        BEGIN

                              DECLARE @valDiff DECIMAL(18,2);

                              SET @valDiff = ((@rangevalue2 - @rangevalue1) + 0.01);

                              SET @valCount = (@valCount +  @valDiff);

 

                              IF @rangevalue1 < @minVal OR @rangevalue1 >= @maxVal  OR @rangevalue2 >= @maxVal --OR @datevalue2 < @minVal

                              BEGIN

                                    CLOSE threshold_cursor

                                    DEALLOCATE threshold_cursor

 

                                    CLOSE loop_cursor2

                                    DEALLOCATE loop_cursor2

 

                                    SET @rangeValid = 0;

                                    GOTO fnExit;

                              END

                        END

 

                        FETCH NEXT FROM threshold_cursor INTO @rangetype, @rangevalue1, @rangevalue2

                  END

 

                  CLOSE threshold_cursor

                  DEALLOCATE threshold_cursor

 

                  IF @valCount <> @valToValidate

                  BEGIN

                        CLOSE loop_cursor2

                        DEALLOCATE loop_cursor2

 

                        SET @rangeValid = 0;

                        GOTO fnExit;

                  END

            END

            ELSE

            BEGIN

                  SET @count = (select count([mileagethresholdid]) from mileage_thresholds where mileagedateid = @mdId and rangetype = 3)

                  IF @count = 0

                  BEGIN

                        CLOSE loop_cursor2

                        DEALLOCATE loop_cursor2

 

                        SET @rangeValid = 0;

                        GOTO fnExit;

                  END

            END

            FETCH NEXT FROM loop_cursor2 INTO @mdId

      END

      CLOSE loop_cursor2

      DEALLOCATE loop_cursor2

 

fnExit:

      RETURN @rangeValid;

 

END





CREATE PROCEDURE [dbo].[ApiBatchSaveEmployeeRole] @list ApiBatchSaveEmployeeRoleType READONLY
AS
BEGIN
 DECLARE @index BIGINT
 DECLARE @count BIGINT
 DECLARE @employeeid INT
 DECLARE @itemroleid INT
 DECLARE @order INT
 DECLARE @tmp TABLE (
  tmpID BIGINT
  ,employeeid BIGINT
  ,itemroleid BIGINT
  )

 INSERT @tmp
 SELECT ROW_NUMBER() OVER (
   ORDER BY employeeid
    ,itemroleid
   )
  ,employeeid
  ,itemroleid
 FROM @list

 SELECT @count = count(*)
 FROM @tmp

 SET @index = 1

 WHILE @index <= @count
 BEGIN
  SELECT TOP 1 @employeeid = employeeid
   ,@itemroleid = itemroleid
  FROM @tmp
  WHERE tmpID = @index;

  SELECT TOP 1 @order = @order
  FROM @list
  WHERE employeeid = @employeeid
   AND itemroleid = itemroleid

  SET @order = (
    SELECT isnull(max([order]), 0)
    FROM employee_roles
    WHERE employeeid = @employeeID
    )

  IF @order = 0
   SET @order = 1

  IF NOT EXISTS (
    SELECT *
    FROM employee_roles
    WHERE employeeid = @employeeid
     AND itemroleid = @itemroleid
    )
  BEGIN
   INSERT INTO employee_roles (
    employeeid
    ,itemroleid
    ,[order]
    )
   VALUES (
    @employeeid
    ,@itemroleid
    ,@order
    )
  END

  SET @index = @index + 1
 END

 RETURN 0;
END


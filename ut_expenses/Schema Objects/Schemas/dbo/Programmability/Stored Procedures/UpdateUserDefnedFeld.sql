CREATE PROCEDURE [dbo].[UpdateUserDefinedField] (@BaseTablePKID UNIQUEIDENTIFIER,
                                                @BaseTableName NVARCHAR(100),
                                                @FieldName     NVARCHAR(20),
                                                @FieldID       UNIQUEIDENTIFIER,
                                                @NewValue      NVARCHAR(2000) = NULL,
                                                @NewValueDate  DATETIME = NULL,
                                                @Id            INT,
                                                @ElementId     INT,
                                                @Record        NVARCHAR(255),
                                                @EmployeeID    INT,
                                                @DelegateId    INT)
AS
    DECLARE @PKColumnName NVARCHAR(20)
    DECLARE @OldValue NVARCHAR(2000);
    DECLARE @OldValueDate DATETIME;
    DECLARE @SQL NVARCHAR(255);

    SELECT @PKColumnName = field
    FROM   fields
    WHERE  fieldid = @BaseTablePKID

    IF (@NewValueDate IS NOT NULL)
      BEGIN
          SET @SQL = 'SELECT @OldValueDate =  ' + @FieldName
                     + ' FROM ' + @BaseTableName + ' WHERE '
                     + @PKColumnName + ' IN ('
                     + CONVERT (NVARCHAR(10), @Id) + ')'

          EXEC Sp_executesql
            @SQL,
            N'@OldValueDate DATETIME OUTPUT',
            @OldValueDate OUTPUT
     
          SET @OldValue = CONVERT(NVARCHAR(24), @oldValueDate, 120)
          SET @NewValue = CONVERT(NVARCHAR(24), @NewValueDate, 120)
		  
      END
    ELSE
      BEGIN
          SET @SQL = 'SELECT @OldValue =  ' + @FieldName + ' FROM '
                     + @BaseTableName + ' WHERE ' + @PKColumnName
                     + ' IN (' + CONVERT (NVARCHAR(10), @Id) + ')'

          EXEC Sp_executesql
            @SQL,
            N'@OldValue NVARCHAR(2000) OUTPUT',
            @OldValue OUTPUT
      END

    IF (@OldValue <> @NewValue)
      DECLARE @SQLUpdate NVARCHAR(2500);

    SET @SQLUpdate ='UPDATE ' + @BaseTableName + ' SET ' + @FieldName
                    + '=' + '''' + @NewValue + '''' + ' WHERE '
                    + @PKColumnName + '='
                    + CONVERT(NVARCHAR(10), @Id)

    EXEC Sp_executesql
      @SQLUpdate

    EXEC Addupdateentrytoauditlog
      @EmployeeID,
      @DelegateId,
      @ElementId,
      @Id,
      @FieldID,
      @oldValue,
      @NewValue,
      @Record,
      NULL;

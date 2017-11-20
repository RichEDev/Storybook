CREATE PROCEDURE [dbo].[GetHeirachy]
	@employeeid int ,
	@matchFieldID uniqueidentifier
AS
	DECLARE @fieldname nvarchar(200);
	DECLARE @tablename nvarchar(200);
	DECLARE @tableid UNIQUEIDENTIFIER;
	DECLARE @temptable Employees
	DECLARE @employeeCount int = 0;
	DECLARE @lastEmployeeCount int = 1;
	
	SELECT @fieldname = field, @tableid = tableid from fields where fieldid = @matchFieldID;
	SELECT @tablename = tablename from tables where tableid = @tableid;
	
	CREATE TABLE #Heirachy	( 	[employeeid] [int], 	[checked] bit DEFAULT (0), 	PRIMARY KEY CLUSTERED ([employeeid] ASC) WITH (IGNORE_DUP_KEY = ON));

	INSERT INTO #Heirachy (employeeid) VALUES (@employeeid)


	DECLARE @sql NVARCHAR(MAX) = 'INSERT INTO #Heirachy (employeeid) SELECT ' + @tablename + '.[employeeid] FROM ' + @tablename + ' WHERE ' + @tablename + '.' + @fieldname + '  IN (SELECT employeeid FROM #Heirachy)';

	WHILE @employeeCount <> @lastEmployeeCount
	BEGIN
		SET @lastEmployeeCount = @employeeCount	
		EXEC sp_executesql @sql;
		SET @employeeCount = (SELECT distinct count(employeeid) FROM #Heirachy)
	END

	SELECT employeeid, checked FROM #Heirachy;

RETURN 0
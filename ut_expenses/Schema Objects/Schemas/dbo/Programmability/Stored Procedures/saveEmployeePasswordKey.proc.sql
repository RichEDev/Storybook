






CREATE PROCEDURE [dbo].[saveEmployeePasswordKey]
	@employeeID int,
	@passwordKeyType tinyint,
	@sendOnDate datetime,
	@uniqueKey nvarchar(MAX)
AS 

	declare @entityID int;
	declare @title1 nvarchar(500);
	declare @recordTitle nvarchar(2000);
	select @title1 = username from employees where employeeID = @employeeID;
	select @recordTitle = (select 'User ' + @title1 + ' passwordkey');

	INSERT INTO dbo.employeePasswordKeys (employeeID, uniqueKey, createdOn, sendOnDate, sendType) VALUES (@employeeID, @uniqueKey, getutcdate(), @sendOnDate, @passwordKeyType);
	set @entityID = scope_identity();

	





-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[authenticateForKnowledgeBase] 
	@companyID nvarchar(50),
	@username nvarchar(50),
	@password nvarchar(250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	
	
	declare @count int
	
    -- Insert statements for procedure here
	set @count = (select count(*) from registeredusers where companyid = @companyID)
	if (@count = 0)
		return -1
		
	declare @hostname nvarchar(50)
	declare @dbname nvarchar(100)
	select @hostname = hostname, @dbname = dbname from registeredusers inner join databases on databases.databaseid = registeredusers.dbserver where companyid = @companyid
	declare @sql nvarchar(max)
	DECLARE @ParmDefinition nvarchar(500);
	if @hostname = '192.168.101.127' --must be changed to the hostname of this server
				set @hostname = ''
			else
				set @hostname = '[' + @hostname + '].';
				
	set @parmdefinition = '@inUsername nvarchar(50), @inPassword nvarchar(250), @countOut int output'
	set @sql = 'select @countOut = count(*) from ' + @hostname + '[' + @dbname + '].dbo.employees where username = @inUsername and password = @inPassword '
	
	execute sp_executesql @sql, @parmdefinition, @inUsername = @username, @inPassword = @password, @countOut = @count output
	
	return @count
END
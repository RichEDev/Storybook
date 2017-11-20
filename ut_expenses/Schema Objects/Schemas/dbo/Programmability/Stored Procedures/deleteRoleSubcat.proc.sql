

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteRoleSubcat]
	@subcatid int,
	@roleid int,
	@employeeID INT,
	@delegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @rolename varchar(500);
	declare @subcatname varchar(500);
	declare @recordTitle nvarchar(2000);

	select @rolename = rolename from item_roles where itemroleid = @roleid;
	select @subcatname = subcat from subcats where subcatid = @subcatid;
	set @recordTitle = (select 'Item Role ' + @rolename + ' - Subcat ' + @subcatname);

    -- Insert statements for procedure here
	delete from rolesubcats where subcatid = @subcatid and roleid = @roleid;

	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 37, @roleid, @recordTitle, null;
END



-- =============================================
-- Author:		Paul Griffiths
-- Create date: 20-07-2011
-- Description:	Gets the audience table name from the tableid, then pulls the assigned audience id's
-- =============================================
CREATE PROCEDURE dbo.[getAudienceIDs]
	@tableid uniqueidentifier,
	@parentid int
AS
BEGIN
declare @tablename nvarchar(500);
declare @sql nvarchar(max);
declare @sParentId nvarchar(10);

set @tablename = (select tablename from tables where tableid=@tableid)
set @sParentId = (select cast(@parentid as nvarchar(10)))
set @sql = 'select audienceid from [' + @tablename + '] where parentid = ' + @sParentId;
exec(@sql);
END

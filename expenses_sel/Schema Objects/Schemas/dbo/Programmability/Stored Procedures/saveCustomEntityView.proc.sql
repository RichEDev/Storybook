CREATE PROCEDURE [dbo].[saveCustomEntityView]
	@viewid INT,
@entityid int,
@viewname nvarchar(100),
@description nvarchar(4000),
@date DateTime,
@userid INT,
@menuid INT,
@allowadd BIT,
@addformid INT,
@allowedit BIT,
@editformid INT,
@allowdelete bit,
@SortColumn uniqueidentifier,
@SortOrder tinyint
--,
--@CUemployeeID INT,
--@CUdelegateID INT
	AS

DECLARE @count INT;
IF (@viewid = 0)
begin
	SET @count = (SELECT COUNT(viewid) FROM [custom_entity_views] WHERE entityid = @entityid AND view_name = @viewname);
	IF @count > 0
		RETURN -1;
		
	insert into custom_entity_views (entityid, view_name, [description], createdby, createdon, menuid, [allowadd], [add_formid], [allowedit], [edit_formid], [allowdelete], [SortColumn], [SortOrder]) values (@entityid, @viewname, @description, @userid, @date, @menuid, @allowadd, @addformid, @allowedit, @editformid, @allowdelete, @SortColumn, @SortOrder)
	SET @viewid = SCOPE_IDENTITY();
end
else
BEGIN
	SET @count = (SELECT COUNT(viewid) FROM [custom_entity_views] WHERE entityid = @entityid AND view_name = @viewname AND viewid <> @viewid);
	IF @count > 0
		RETURN -1;
	update custom_entity_views set view_name = @viewname, [description] = @description, modifiedby = @userid, modifiedon = @date, menuid = @menuid, allowadd = @allowadd, [add_formid] = @addformid, [allowedit] = @allowedit, [edit_formid] = @editformid, [allowdelete] = @allowdelete, SortColumn = @SortColumn, SortOrder = @SortOrder where viewid = @viewid
	
end

return @viewid

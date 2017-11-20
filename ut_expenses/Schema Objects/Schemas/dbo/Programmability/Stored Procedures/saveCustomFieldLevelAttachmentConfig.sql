CREATE PROCEDURE [dbo].[saveCustomFieldLevelAttachmentConfig]
@basetable uniqueidentifier,
@destinationKey uniqueidentifier,
@entityName nvarchar(250)
as
 BEGIN
	declare @jointableid uniqueidentifier
	declare @joinbreakdownid uniqueidentifier
	declare @order tinyint
	declare @tableid uniqueidentifier
	declare @sourcetable uniqueidentifier
	declare @joinkey uniqueidentifier
	declare @desc nvarchar(25)
	declare @amendedon datetime

	set @jointableid = NEWID();
	set @tableid = '92A15F16-BCEC-4666-8478-1EF83ED3D076' 
	set @desc = @entityName + ' to customEntityImageData'
	set @amendedon = GETDATE()
	set @order = 1
	set @joinkey = '953D22CE-A7C1-4512-867C-BE601A78FE09' 
 END
 
 IF NOT EXISTS (SELECT jointableid FROM [customJoinTables] WHERE basetableid = @basetable AND tableid = @tableid)
 BEGIN
	INSERT INTO [customJoinTables] (jointableid, tableid, basetableid, [description], amendedon)
	VALUES (@jointableid, @tableid, @basetable, @desc, @amendedon);
 END

IF NOT EXISTS (SELECT joinbreakdownid FROM [customJoinBreakdown] WHERE sourcetable = @basetable and destinationkey = @destinationKey)
 BEGIN
	Select @jointableid = jointableid from customJoinTables WHERE basetableid = @basetable AND tableid = @tableid 
	INSERT INTO [customJoinBreakdown] (jointableid, [order], tableid, sourcetable, joinkey, destinationkey, amendedon)
	VALUES (@jointableid, @order, @tableid, @basetable, @joinkey, @destinationKey, @amendedon)
  END


GO



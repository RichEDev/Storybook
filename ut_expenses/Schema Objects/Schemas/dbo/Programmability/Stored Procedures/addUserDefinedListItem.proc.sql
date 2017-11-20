CREATE PROCEDURE [dbo].[addUserDefinedListItem]
	@userdefineid INT,
	@valueid int,
	@order INT,
	@item NVARCHAR(50),
	@archived bit,
	@CUemployeeID INT,
	@CUdelegateID INT
AS
BEGIN
	declare @count int;
	declare @returnID int;
	set @returnID = 0;
	
	declare @title1 nvarchar(500);
	select @title1 = attribute_name from userdefined where userdefineid = @userdefineid;

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'UDF ' + @title1 + ' List item - ' + @item);
	
	if @valueid = 0
	begin
		set @count = (select count(*) from userdefined_list_items where userdefineid = @userdefineid and item = @item);
		
		if(@count = 0)
		begin
			-- SET NOCOUNT ON added to prevent extra result sets from
			-- interfering with SELECT statements.
			SET NOCOUNT ON;

			-- Insert statements for procedure here
			INSERT INTO [userdefined_list_items] (
				[userdefineid],
				[item],
				[order],
				[archived]
			) VALUES ( @userdefineid, @item, @order, @archived);

			exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, @recordTitle, null;
			
			set @returnID = SCOPE_IDENTITY();
		end
	end
	else
	begin
		set @count = (select count(*) from userdefined_list_items where userdefineid = @userdefineid and item = @item and valueid <> @valueid);
		
		if @count = 0
		begin
			declare @oldorder int;
			declare @olditem nvarchar(50);
			declare @oldarchived bit;
			
			select @oldorder = [order], @olditem = item, @oldarchived = archived from userdefined_list_items where valueid = @valueid;
			
			UPDATE userdefined_list_items set [order] = @order, item = @item, archived = @archived where valueid = @valueid;
			
			if @oldorder <> @order
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, null, @oldorder, @order, @recordTitle, null 
			if @olditem <> @item
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, null, @oldorder, @order, @recordTitle, null 
			if @oldarchived <> @archived
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, null, @oldarchived, @archived, @recordTitle, null 
				
			set @returnID = @valueid;
		end
	end
	
	return @returnID;
END

-- BEGIN TRANSACTION CHECK
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
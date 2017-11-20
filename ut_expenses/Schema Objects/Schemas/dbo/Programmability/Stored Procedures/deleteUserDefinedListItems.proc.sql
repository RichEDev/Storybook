
CREATE PROCEDURE [dbo].[deleteUserDefinedListItems]
	@valueid int,
	@userdefineid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @title1 nvarchar(500);
	select @title1 = attribute_name from userdefined where userdefineid = @userdefineid;

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'UDF ' + @title1 + ' List items');

    -- Insert statements for procedure here
	DELETE FROM [userdefined_list_items] WHERE valueid = @valueid;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, @recordTitle, null;
END


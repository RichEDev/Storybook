


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteUserdefined] 
	-- Add the parameters for the stored procedure here
	@userdefineid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	declare @recordTitle nvarchar(2000);
	select @recordTitle = attribute_name from userdefined where userdefineid = @userdefineid;

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @tablename nvarchar(50);
	declare @fieldname nvarchar(100);
	declare @sql nvarchar(max);
    -- Insert statements for procedure here
    BEGIN TRANSACTION
		select @tablename = tablename from tables inner join userdefined on userdefined.tableid = tables.tableid where userdefined.userdefineid = @userdefineid;
		select @fieldname = field from fields inner join userdefined on userdefined.fieldid = fields.fieldid where userdefined.userdefineid = @userdefineid;
		set @sql = 'alter table [' + @tablename + '] drop column [' + @fieldname + ']';
		EXECUTE sp_executesql @sql
		delete from userdefined where userdefineid = @userdefineid;

		exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, @recordTitle, null;
	
	IF @@ERROR <> 0
		BEGIN 
			ROLLBACK
			RETURN 0
		END
	COMMIT
		
	RETURN 1
END




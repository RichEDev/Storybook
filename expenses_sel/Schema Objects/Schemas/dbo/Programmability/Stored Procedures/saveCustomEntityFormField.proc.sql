CREATE PROCEDURE [dbo].[saveCustomEntityFormField] (@formid int, @attributeid INT, @sectionid int, @readonly bit, @row TINYINT, @column tinyint)--,@CUemployeeID INT,@CUdelegateID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO custom_entity_form_fields (formid, attributeid, sectionid, [readonly], [row], [column]) VALUES (@formid, @attributeid,  @sectionid, @readonly, @row, @column);
	
END


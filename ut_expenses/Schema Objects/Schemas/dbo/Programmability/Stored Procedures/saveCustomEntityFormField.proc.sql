CREATE PROCEDURE [dbo].[saveCustomEntityFormField] (@formid int, @attributeid INT, @sectionid int, @readonly bit, @row TINYINT, @column tinyint, @labeltext nvarchar(250), @defaultValue nvarchar(MAX), @isMandatory bit = null)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [customEntityFormFields] (formid, attributeid, sectionid, [readonly], [row], [column], labelText, DefaultValue,FormMandatory) 
	VALUES (@formid, @attributeid,  @sectionid, @readonly, @row, @column, @labeltext, @defaultValue, @isMandatory);
	
END


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteCustomEntityFormDesign] (@formid INT,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM [custom_entity_form_fields] WHERE [formid] = @formid;
	DELETE FROM [custom_entity_form_sections] WHERE [formid] = @formid;
	DELETE FROM [custom_entity_form_tabs] WHERE [formid] = @formid;
END


CREATE PROCEDURE [dbo].[deleteCustomEntityFormDesign] (@formid INT,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM [customEntityFormFields] WHERE [formid] = @formid;
	DELETE FROM [customEntityFormSections] WHERE [formid] = @formid;
	DELETE FROM [customEntityFormTabs] WHERE [formid] = @formid;
END


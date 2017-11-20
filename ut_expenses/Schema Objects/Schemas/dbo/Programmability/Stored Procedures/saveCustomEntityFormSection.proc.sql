CREATE PROCEDURE [dbo].[saveCustomEntityFormSection] (@formid int, @header NVARCHAR(100), @order TINYINT, @tabid int)--,@CUemployeeID INT,@CUdelegateID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [customEntityFormSections] (formid, header_caption, [order], tabid) VALUES (@formid, @header, @order, @tabid);
	RETURN SCOPE_IDENTITY();
END

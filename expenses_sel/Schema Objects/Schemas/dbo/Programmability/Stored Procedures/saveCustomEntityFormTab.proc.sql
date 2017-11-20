﻿CREATE PROCEDURE [dbo].[saveCustomEntityFormTab] (@formid int, @header NVARCHAR(100), @order tinyint)--,@CUemployeeID INT,@CUdelegateID INT)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO custom_entity_form_tabs (formid, header_caption, [order]) VALUES (@formid, @header, @order);
	RETURN SCOPE_IDENTITY();
END

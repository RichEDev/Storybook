CREATE PROC [dbo].[GetChildFieldValues]
@parentControlId NVARCHAR(100),
@formId NVARCHAR(100)
AS
BEGIN

SELECT attributeid as FieldToBuild FROM fieldFilters WHERE [value]=@parentControlId AND formid =@formId AND isParentFilter =1

END
GO
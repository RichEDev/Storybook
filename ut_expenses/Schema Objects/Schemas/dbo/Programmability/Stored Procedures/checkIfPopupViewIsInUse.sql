Create PROCEDURE [dbo].[checkIfPopupViewIsInUse] (@viewid INT)
AS
BEGIN

Declare @count int

Select @count=COUNT([defaultPopupView]) from [customEntities] where [defaultPopupView] = @viewid and enablePopupWindow = 1

return @count

END



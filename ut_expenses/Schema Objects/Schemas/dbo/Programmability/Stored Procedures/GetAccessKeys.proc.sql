CREATE PROCEDURE [dbo].[GetAccessKeys] 

AS
BEGIN
	SELECT KeyID, [Key], EmployeeID, DeviceID, Active, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM accessKeys
END

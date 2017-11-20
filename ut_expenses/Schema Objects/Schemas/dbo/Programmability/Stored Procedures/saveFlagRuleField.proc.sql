CREATE PROCEDURE [dbo].[saveFlagRuleField]
 -- Add the parameters for the stored procedure here
 @flagID INT,
 @fieldID UNIQUEIDENTIFIER,
 @employeeID INT,
 @delegateID INT
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

    INSERT INTO dbo.flagAssociatedFields ( flagID, fieldID ) VALUES  (@flagID, @fieldID)
    
    declare @description nvarchar(max)
    declare @fielddescription nvarchar(1000)
    declare @flagDescription nvarchar(2000)
    
    set @flagDescription = (select description from flags where flagID = @flagID)
    set @fieldDescription = (select description from fields where fieldid = @fieldID)
    
    set @description = @flagDescription + ' - Field ' + @fielddescription + ' associated to the flag rule.'
    exec addInsertEntryToAuditLog @employeeID, @delegateID, 33, @flagID, @description, null;
    
END

GO

CREATE PROCEDURE [dbo].[deleteCustomEntityForm] (@formid INT,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @description nvarchar(100);
	set @description = (select custom_entities.entity_name + ' : ' + form_name from custom_entity_forms inner join custom_entities on custom_entities.entityid = custom_entity_forms.entityid where formid = @formid)
    
	DELETE FROM [custom_entity_forms] WHERE formid = @formid;
	
	exec addDeleteEntryToAuditLog @CUemployeeId, @CUdelegateID, 133, @formid, @description, null;
END

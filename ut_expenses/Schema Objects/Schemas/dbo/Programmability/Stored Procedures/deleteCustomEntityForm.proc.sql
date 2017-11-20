CREATE PROCEDURE [dbo].[deleteCustomEntityForm] (@formid INT,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT viewFormSelectionMappingId FROM customEntityViewFormSelectionMappings WHERE formId = @formid)
		RETURN -2;

	IF EXISTS(SELECT formid FROM CustomEntityForms WHERE formid = @formid AND BuiltIn = 1)
		RETURN -3;

	DECLARE @description nvarchar(100);
	set @description = (select [customEntities].entity_name + ' : ' + form_name from [customEntityForms] inner join [customEntities] on [customEntities].entityid = [customEntityForms].entityid where formid = @formid)
	
	DECLARE @formused int = 0;
	
	select @formused = count(*) FROM customEntityViews WHERE add_formid = @formid OR edit_formid = @formid
	if @formused = 0
		begin
			DELETE FROM [customEntityForms] WHERE formid = @formid;
			exec addDeleteEntryToAuditLog @CUemployeeId, @CUdelegateID, 133, @formid, @description, null;
			return 0;
		end
	else
		return -1;
END
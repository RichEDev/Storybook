




CREATE PROCEDURE [dbo].[saveWorkflow]

	@workflowid int,
	@workflowname nvarchar(250),
	@description nvarchar(4000),
	@canbechildworkflow bit,
	@runoncreation bit,
	@runonchange bit,
	@runondelete bit,
	@workflowtype int,
	@activeUser int,
	@baseTableID uniqueidentifier,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	DECLARE @count INT;
	DECLARE @dateModified DateTime;
	SET @dateModified = getdate();
	IF (@workflowid = 0)
		BEGIN
			SET @count = (SELECT COUNT(*) FROM workflows WHERE workflowName = @workflowname);
		
			IF @count > 0
				RETURN -1;

			INSERT INTO workflows (workflowName, description, canBeChildWorkflow, runOnCreation, runOnChange, runOnDelete, workflowType, createdon, createdby, baseTableID) VALUES (@workflowname, @description, @canbechildworkflow, @runoncreation, @runonchange, @runondelete, @workflowtype, @dateModified, @activeUser, @baseTableID);
			SET @workflowid = scope_identity();

			exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowid, @workflowname, null;
		END
	ELSE
		BEGIN

			SET @count = (SELECT COUNT(*) FROM workflows WHERE workflowName = @workflowname AND workflowID <> @workflowid);
			
			IF @count > 0
				RETURN -1;

			declare @oldworkflowname nvarchar(250);
			declare @olddescription nvarchar(4000);
			declare @oldcanbechildworkflow bit;
			declare @oldrunoncreation bit;
			declare @oldrunonchange bit;
			declare @oldrunondelete bit;
			declare @oldworkflowtype int;
			declare @oldbaseTableID uniqueidentifier;
			select @oldworkflowname = workflowName, @olddescription = [description], @oldcanbechildworkflow = canBeChildWorkflow, @oldrunoncreation = runOnCreation, @oldrunonchange = runOnChange, @oldrunondelete = runOnDelete, @oldworkflowtype = workflowType, @oldbaseTableID = baseTableID from workflows WHERE workflowID=@workflowid

			UPDATE workflows SET workflowName=@workflowname, [description]=@description, canBeChildWorkflow=@canbechildworkflow, runOnCreation=@runoncreation, runOnChange=@runonchange, runOnDelete=@runondelete, workflowType=@workflowtype, modifiedon=@dateModified, modifiedby=@activeUser, baseTableID=@baseTableID WHERE workflowID=@workflowid;

			if @oldworkflowname <> @workflowname
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowid, '6f697bb8-7917-4579-8db7-9d4569220780', @oldworkflowname, @workflowname, @workflowname, null;
			if @olddescription <> @description
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowid, '4cd5934f-5138-4de2-93b5-264fcddbd617', @olddescription, @description, @workflowname, null;
			if @oldcanbechildworkflow <> @canbechildworkflow
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowid, '22555e98-38a6-46aa-b3fa-9d571a26a93e', @oldcanbechildworkflow, @canbechildworkflow, @workflowname, null;
			if @oldrunoncreation <> @runoncreation
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowid, 'c3330a03-0b13-4bbc-8dad-fed8f57fcf9d', @oldrunoncreation, @runoncreation, @workflowname, null;
			if @oldrunonchange <> @runonchange
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowid, 'bf986548-7e55-42a2-94dc-cf1810fa01b1', @oldrunonchange, @runonchange, @workflowname, null;
			if @oldrunondelete <> @runondelete
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowid, '9d27aab5-18c5-4697-a674-b851c97dc3af', @oldrunondelete, @runondelete, @workflowname, null;
			if @oldworkflowtype <> @workflowtype
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowid, '77a71980-3d40-4d68-8977-bb44bfb856c7', @oldworkflowtype, @workflowtype, @workflowname, null;
			if @oldbaseTableID <> @baseTableID
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowid, '5a02f279-d809-4b65-a2ba-948d3e3c540f', @oldbaseTableID, @baseTableID, @workflowname, null;
		END
RETURN @workflowid;




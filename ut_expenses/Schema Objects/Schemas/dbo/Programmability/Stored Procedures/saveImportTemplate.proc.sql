



CREATE PROCEDURE [dbo].[saveImportTemplate]

@templateID int,
@templateName nvarchar(150),
@applicationType tinyint,
@isAutomated bit,
@NHSTrustID int,
@date datetime,
@userid int,
@CUemployeeID INT,
@CUdelegateID INT,
@SignOffOwnerFieldId uniqueidentifier,
@LineManagerFieldId uniqueidentifier
AS

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Import Template - ' + @templateName);

if @templateID = 0
BEGIN
	INSERT INTO importTemplates
		(templateName, applicationType, isAutomated, NHSTrustID, createdOn, createdBy, SignOffOwnerFieldId, LineManagerFieldId)
	VALUES
		(@templateName, @applicationType, @isAutomated, @NHSTrustID, @date, @userid, @SignOffOwnerFieldId, @LineManagerFieldId)
	
	SET @templateID = scope_identity();

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 27, @templateID, @recordTitle, null;
END
else
BEGIN
	declare @oldtemplateName nvarchar(150);
	declare @oldapplicationType tinyint;
	declare @oldisAutomated bit;
	declare @oldNHSTrustID int;
	select @oldtemplateName = templateName, @oldapplicationType = applicationType, @oldisAutomated = isAutomated, @oldNHSTrustID = NHSTrustID from importTemplates WHERE templateID = @templateID;

	UPDATE
		importTemplates
	SET
		templateName = @templateName,
		applicationType = @applicationType,
		isAutomated = @isAutomated,
		NHSTrustID = @NHSTrustID,
		modifiedOn = @date,
		modifiedBy = @userid,
		SignOffOwnerFieldId = @SignOffOwnerFieldId,
		LineManagerFieldId = @LineManagerFieldId
	WHERE
		templateID = @templateID;

	if @oldtemplateName <> @templateName
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 27, @templateID, '41a230b6-f451-4ca0-9b89-4004ebabf607', @oldtemplateName, @templateName, @recordtitle, null;
	if @oldapplicationType <> @applicationType
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 27, @templateID, '3d51ebb3-9e9e-4fb2-8e57-05dda124d594', @oldapplicationType, @applicationType, @recordtitle, null;
	if @oldisAutomated <> @isAutomated
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 27, @templateID, 'a1dc7095-1893-446c-b511-b55e652a3b9c', @oldisAutomated, @isAutomated, @recordtitle, null;
	if @oldNHSTrustID <> @NHSTrustID
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 27, @templateID, '6bb30cc5-c050-4177-957e-caa3b99d777b', @oldNHSTrustID, @NHSTrustID, @recordtitle, null;
END

return @templateID






 

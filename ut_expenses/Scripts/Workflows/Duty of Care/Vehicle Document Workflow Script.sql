DELETE
FROM workflows
WHERE workflowName = 'Notify Claimant and Approver On Duty Of Care Documents Review Status and requests'
GO
SET NUMERIC_ROUNDABORT OFF;
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON;
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors;
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#stepIds')) DROP TABLE #stepIds;
CREATE TABLE #tmpErrors (Error int);
GO
IF OBJECT_ID('tempdb..#GetAttributeListValue') IS NOT NULL DROP PROCEDURE #GetAttributeListValue
GO
CREATE PROCEDURE #GetAttributeListValue (@SystemCustomEntityAttributeID uniqueidentifier, @ValueText nvarchar(200)) AS
BEGIN
DECLARE @attributeId int
DECLARE @resultValue int = 0
SELECT @attributeId = attributeid from customEntityAttributes where SystemCustomEntityAttributeID = @SystemCustomEntityAttributeID
  SELECT top 1 @resultValue = valueid from  customEntityAttributeListItems where attributeid = @attributeId and item = @ValueText
  RETURN @resultValue
END
GO
SET XACT_ABORT ON;
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;
PRINT '------- DoC Vehicle Documents Workflow Migration Script Customer -------';
GO

IF EXISTS (
		SELECT *
		FROM workflows
		WHERE workflowName = 'Notify Claimant and Approver On Duty Of Care Documents Review Status and requests'
		)
	DELETE
	FROM workflows
	WHERE workflowName = 'Notify Claimant and Approver On Duty Of Care Documents Review Status and requests'


declare @tempworkflowname nvarchar(250)
declare @relatedStepDescription nvarchar(500)
declare @tempbasetableid uniqueidentifier
declare @tempWorkflowID int
declare @tempparentstepid int
declare @tempdescription nvarchar(150)
declare @tempworkflowdescription nvarchar(150)
declare @count int
declare @tablename nvarchar(500)
declare @fieldname nvarchar(250)
declare @tempcondition tinyint
declare @tempvalue1 nvarchar(200)
declare @tempvalue2 nvarchar(200)
declare @criteriaFieldID uniqueidentifier
declare @tempemailtemplatename nvarchar(250)
declare @employeeid int
declare @tempWorkflowStepID int
declare @parentdescription nvarchar(150)
declare @tempemailtemplateid int
declare @username nvarchar(250)
declare @relatedStep int
declare @formula nvarchar(500)
declare @tempworkflowsteptomoveto nvarchar(500)
declare @tempsteptomoveto nvarchar(500)
declare  @runsubworkflowid int
declare  @runsubworkflowname nvarchar(500)
 declare @tempTaxValue nvarchar(200)
 declare @tempInsuranceValue nvarchar(200)
  declare @tempMOTValue nvarchar(200)
 declare @tempServiceValue nvarchar(200) 
 declare @tempBreakDownValue nvarchar(200)
 declare @tempAwaitingReviewValue nvarchar(200)
 declare @tempInvalidatedValue nvarchar(200)
 declare @tempFailedValue nvarchar(200)
 declare @tempOkValue nvarchar(200)
create table #stepIds
(
description nvarchar(150),
id int
)
set @username = 'greg'
set @employeeid = (select employeeid from employees where username = @username)
set @tempworkflowname = 'Notify Claimant and Approver On Duty Of Care Documents Review Status and requests'
set @tempbasetableid = '28D592D7-4596-49C4-96B8-45655D8DF797'
exec @tempworkflowID = saveWorkflow @workflowid = 0, @workflowname = @tempworkflowname, @description = @tempworkflowdescription, @runoncreation = 1, @runonchange = 1, @runondelete = 0, @basetableID = @tempbasetableid, @canbechildworkflow = 0, @workflowtype = 5, @activeuser = @employeeid, @CUemployeeID = @employeeid, @CUdelegateID = null
 exec @tempTaxValue =  #GetAttributeListValue 'F190DA6F-DC4E-4B54-92C2-1EB68451EFC9','Tax' 
 exec @tempInsuranceValue =  #GetAttributeListValue 'F190DA6F-DC4E-4B54-92C2-1EB68451EFC9','Insurance' 
  exec @tempMOTValue =  #GetAttributeListValue 'F190DA6F-DC4E-4B54-92C2-1EB68451EFC9','MOT' 
 exec @tempServiceValue =   #GetAttributeListValue 'F190DA6F-DC4E-4B54-92C2-1EB68451EFC9','Service' 
 exec @tempBreakDownValue = #GetAttributeListValue 'F190DA6F-DC4E-4B54-92C2-1EB68451EFC9','Breakdown Cover' 
 exec @tempAwaitingReviewValue =   #GetAttributeListValue '19EC2F1F-0EF4-465B-B6EE-D2CE2FA7021B','Awaiting Review' 
 exec @tempInvalidatedValue = #GetAttributeListValue '19EC2F1F-0EF4-465B-B6EE-D2CE2FA7021B','Invalidated' 
 exec @tempFailedValue = #GetAttributeListValue '19EC2F1F-0EF4-465B-B6EE-D2CE2FA7021B','Reviewed-Failed' 
 exec @tempOkValue =  #GetAttributeListValue '19EC2F1F-0EF4-465B-B6EE-D2CE2FA7021B','Reviewed-OK' 
if @tempworkflowID = -1
begin
    print 'The workflow name ' + @tempworkflowname + ' already exists.'
    drop table #stepids
rollback transaction
    return
end
else
    print 'Workflow ' + @tempworkflowname + ' created with ID ' + cast(@tempworkflowID as nvarchar)
set @tempparentstepid = 0
print 'Creating workflow step If the review status = Failed'
set @tempdescription = 'If the review status = Failed'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If the review status = Failed'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @tempcondition = 1
set @tempvalue1 = @tempFailedValue
set @tempvalue2 = ''
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the review status = Failed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Check if the user is reviewer'
set @tempdescription = 'Check if the user is reviewer'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Check if the user is reviewer'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Reviewed by'
set @tempcondition = 1
set @tempvalue1 = 'DEB645A2-1A9D-48E2-8801-46E7A69802AC'
set @tempvalue2 = ''
set @criteriaFieldID = '80591948-5246-4d25-b606-c99a93d5f911'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its an insurance document'
set @tempdescription = 'If its an insurance document'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its an insurance document'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempInsuranceValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set the Review Date'
set @tempdescription = 'Set the Review Date'
set @formula = 'TEXT(NOW(),"yyyy-MM-dd HH:mm:ss.fff")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Review date'
set @criteriaFieldID = 'b1ee967d-cea8-4db6-8410-9992478b5be7'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Send Email to Notify claimant if the approver is unhappy with the insurance document'
set @tempdescription = 'Send Email to Notify claimant if the approver is unhappy with the insurance document'
set @tempemailtemplatename = 'Send to the claimant if the approver is unhappy with the vehicle document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a MOT document'
set @tempdescription = 'If its a MOT document'
set @relatedStepDescription = 'If its an insurance document'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a MOT document'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempMOTValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set the Review Date'
set @tempdescription = 'Set the Review Date'
set @formula = 'TEXT(NOW(),"yyyy-MM-dd HH:mm:ss.fff")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Review date'
set @criteriaFieldID = 'b1ee967d-cea8-4db6-8410-9992478b5be7'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Send Email to Notify claimant if the approver is unhappy with the MOT document'
set @tempdescription = 'Send Email to Notify claimant if the approver is unhappy with the MOT document'
set @tempemailtemplatename = 'Send to the claimant if the approver is unhappy with the vehicle document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a service document'
set @tempdescription = 'If its a service document'
set @relatedStepDescription = 'If its an insurance document'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a service document'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempServiceValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set the Review Date'
set @tempdescription = 'Set the Review Date'
set @formula = 'TEXT(NOW(),"yyyy-MM-dd HH:mm:ss.fff")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Review date'
set @criteriaFieldID = 'b1ee967d-cea8-4db6-8410-9992478b5be7'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Send Email to Notify claimant if the approver is unhappy with the service document'
set @tempdescription = 'Send Email to Notify claimant if the approver is unhappy with the service document'
set @tempemailtemplatename = 'Send to the claimant if the approver is unhappy with the vehicle document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a tax document'
set @tempdescription = 'If its a tax document'
set @relatedStepDescription = 'If its an insurance document'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a tax document'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempTaxValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set the Review Date'
set @tempdescription = 'Set the Review Date'
set @formula = 'TEXT(NOW(),"yyyy-MM-dd HH:mm:ss.fff")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Review date'
set @criteriaFieldID = 'b1ee967d-cea8-4db6-8410-9992478b5be7'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Send Email to Notify claimant if the approver is unhappy with the tax document'
set @tempdescription = 'Send Email to Notify claimant if the approver is unhappy with the tax document'
set @tempemailtemplatename = 'Send to the claimant if the approver is unhappy with the vehicle document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a breakdown cover document'
set @tempdescription = 'If its a breakdown cover document'
set @relatedStepDescription = 'If its an insurance document'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a breakdown cover document'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempBreakDownValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set the Review Date'
set @tempdescription = 'Set the Review Date'
set @formula = 'TEXT(NOW(),"yyyy-MM-dd HH:mm:ss.fff")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Review date'
set @criteriaFieldID = 'b1ee967d-cea8-4db6-8410-9992478b5be7'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Send Email to Notify claimant if the approver is unhappy with the vehicle breakdown cover document'
set @tempdescription = 'Send Email to Notify claimant if the approver is unhappy with the vehicle breakdown cover document'
set @tempemailtemplatename = 'Send to the claimant if the approver is unhappy with the vehicle document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @tempparentstepid = 0
print 'Creating workflow step If the review is good'
set @tempdescription = 'If the review is good'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If the review is good'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @tempcondition = 1
set @tempvalue1 = @tempOkValue
set @tempvalue2 = ''
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the review is good'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Assign Approver Check'
set @tempdescription = 'Assign Approver Check'
set @formula = 'IF("[deb645a2-1a9d-48e2-8801-46e7a69802ac]">" ",1,0)'
set @tablename = 'Vehicle Document'
set @fieldname = 'ApproverCheck'
set @criteriaFieldID = 'f5ace912-2c70-4e56-bf56-62a05e0f62ff'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the review is good'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Check if its a new docuemnt uploaded by reviewer setting review to OK'
set @tempdescription = 'Check if its a new docuemnt uploaded by reviewer setting review to OK'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Check if its a new docuemnt uploaded by reviewer setting review to OK'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'ApproverCheck'
set @tempcondition = 1
set @tempvalue1 = '1'
set @tempvalue2 = ''
set @criteriaFieldID = 'f5ace912-2c70-4e56-bf56-62a05e0f62ff'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if its a new docuemnt uploaded by reviewer setting review to OK'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Assign Approver Check'
set @tempdescription = 'Assign Approver Check'
set @formula = 'IF("[80591948-5246-4d25-b606-c99a93d5f911]">" ",1,2)'
set @tablename = 'Vehicle Document'
set @fieldname = 'ApproverCheck'
set @criteriaFieldID = 'f5ace912-2c70-4e56-bf56-62a05e0f62ff'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the review is good'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Check if reviewer is null - data migration'
set @tempdescription = 'Check if reviewer is null - data migration'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Check if reviewer is null - data migration'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'ApproverCheck'
set @tempcondition = 1
set @tempvalue1 = '2'
set @tempvalue2 = ''
set @criteriaFieldID = 'f5ace912-2c70-4e56-bf56-62a05e0f62ff'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if reviewer is null - data migration'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its an insurance migrated document edit'
set @tempdescription = 'If its an insurance migrated document edit'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its an insurance migrated document edit'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempInsuranceValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details - Policy number'
set @tempdescription = 'If claimant modifies the migrated DOC details - Policy number'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details - Policy number'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Policy number_validate'
set @tempcondition = 2
set @tempvalue1 = 'afff4d09-7350-4262-b0f9-ae25de53b7d5'
set @tempvalue2 = ''
set @criteriaFieldID = '27f0f5bf-1071-433f-bbe4-b6f9b4111eea'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Policy number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy policy number to duplicate attribute'
set @tempdescription = 'Copy policy number to duplicate attribute'
set @formula = '("[afff4d09-7350-4262-b0f9-ae25de53b7d5]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Policy number_validate'
set @criteriaFieldID = '27f0f5bf-1071-433f-bbe4-b6f9b4111eea'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Policy number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details - Is commuter travel included?'
set @tempdescription = 'If claimant modifies the migrated DOC details - Is commuter travel included?'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details - Is commuter travel included?'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Is commuter travel included_validate'
set @tempcondition = 2
set @tempvalue1 = '99bfcf34-5a0d-4efc-9c06-e6d2c87ddc4c'
set @tempvalue2 = ''
set @criteriaFieldID = 'a315feb5-efe7-4bfe-9d01-b2d941c716a5'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Is commuter travel included?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Is commuter travel included? to duplicate attribute'
set @tempdescription = 'Copy Is commuter travel included? to duplicate attribute'
set @formula = '("[99bfcf34-5a0d-4efc-9c06-e6d2c87ddc4c]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Is commuter travel included_validate'
set @criteriaFieldID = 'a315feb5-efe7-4bfe-9d01-b2d941c716a5'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Is commuter travel included?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details - Is class 1 business travel included?'
set @tempdescription = 'If claimant modifies the migrated DOC details - Is class 1 business travel included?'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details - Is class 1 business travel included?'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Is class 1 check_validate'
set @tempcondition = 2
set @tempvalue1 = 'b2fa95dd-85df-4e8b-a4d0-2ae46abcc7ca'
set @tempvalue2 = ''
set @criteriaFieldID = 'bad8a1b7-ec39-4793-af86-cc5c859f2953'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Is class 1 business travel included?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Is class 1 business travel included? to duplicate attribute'
set @tempdescription = 'Copy Is class 1 business travel included? to duplicate attribute'
set @formula = '("[b2fa95dd-85df-4e8b-a4d0-2ae46abcc7ca]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Is Class 1 check_validate'
set @criteriaFieldID = 'bad8a1b7-ec39-4793-af86-cc5c859f2953'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Is class 1 business travel included?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details - Cover type'
set @tempdescription = 'If claimant modifies the migrated DOC details - Cover type'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details - Cover type'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Cover type_validate'
set @tempcondition = 2
set @tempvalue1 = '5775929a-f3ac-42e0-be9d-c47fbd430d01'
set @tempvalue2 = ''
set @criteriaFieldID = 'ef872f14-88bf-4c0c-bdee-47d9a2c539d9'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Cover type'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Cover type to duplicate attribute'
set @tempdescription = 'Copy Cover type to duplicate attribute'
set @formula = '("[5775929a-f3ac-42e0-be9d-c47fbd430d01]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Cover type_validate'
set @criteriaFieldID = 'ef872f14-88bf-4c0c-bdee-47d9a2c539d9'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Cover type'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details - Provider'
set @tempdescription = 'If claimant modifies the migrated DOC details - Provider'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details - Provider'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Provider_validate'
set @tempcondition = 2
set @tempvalue1 = '27109f54-4305-4b64-896c-94c6a15b52c0'
set @tempvalue2 = ''
set @criteriaFieldID = 'a5e78abe-a398-473e-8a63-f734d9084e7e'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Provider'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Provider to duplicate attribute'
set @tempdescription = 'Copy Provider to duplicate attribute'
set @formula = '("[27109f54-4305-4b64-896c-94c6a15b52c0]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Provider_validate'
set @criteriaFieldID = 'a5e78abe-a398-473e-8a63-f734d9084e7e'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Provider'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details of vehicle - Document in Insurance'
set @tempdescription = 'If claimant modifies the migrated DOC details of vehicle - Document in Insurance'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details of vehicle - Document in Insurance'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @tempcondition = 2
set @tempvalue1 = '3e9ed916-08c9-4192-ad29-b2103b458d26'
set @tempvalue2 = ''
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details of vehicle - Document in Insurance'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Document to duplicate attribute'
set @tempdescription = 'Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details of vehicle - Document in Insurance'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if reviewer is null - data migration'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a MOT migrated document edit'
set @tempdescription = 'If its a MOT migrated document edit'
set @relatedStepDescription = 'If its an insurance migrated document edit'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a MOT migrated document edit'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempMOTValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details - Do you require an MOT?'
set @tempdescription = 'If claimant modifies the migrated DOC details - Do you require an MOT?'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details - Do you require an MOT?'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Do you require an MOT_validate'
set @tempcondition = 2
set @tempvalue1 = 'f430ae0c-9a46-49c8-8f91-068db4b33c9b'
set @tempvalue2 = ''
set @criteriaFieldID = '8f24a33e-5ce8-4545-9237-e4a9b6697842'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Do you require an MOT?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Do you require an MOT? to duplicate attribute'
set @tempdescription = 'Copy Do you require an MOT? to duplicate attribute'
set @formula = '("[f430ae0c-9a46-49c8-8f91-068db4b33c9b]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Do you require an MOT_validate'
set @criteriaFieldID = '8f24a33e-5ce8-4545-9237-e4a9b6697842'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Do you require an MOT?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details - test number'
set @tempdescription = 'If claimant modifies the migrated DOC details - test number'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details - test number'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Test number_validate'
set @tempcondition = 2
set @tempvalue1 = 'a0a992e3-2e76-4260-b111-5eff4366ae35'
set @tempvalue2 = ''
set @criteriaFieldID = 'ca355e81-30ab-4e51-adc4-9b723e26fe5a'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - test number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy test number to duplicate attribute'
set @tempdescription = 'Copy test number to duplicate attribute'
set @formula = '("[a0a992e3-2e76-4260-b111-5eff4366ae35]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Test number_validate'
set @criteriaFieldID = 'ca355e81-30ab-4e51-adc4-9b723e26fe5a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - test number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details of vehicle - Document in MOT records'
set @tempdescription = 'If claimant modifies the migrated DOC details of vehicle - Document in MOT records'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details of vehicle - Document in MOT records'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @tempcondition = 2
set @tempvalue1 = '3e9ed916-08c9-4192-ad29-b2103b458d26'
set @tempvalue2 = ''
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details of vehicle - Document in MOT records'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Document to duplicate attribute'
set @tempdescription = 'Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details of vehicle - Document in MOT records'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if reviewer is null - data migration'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a service migrated document edit'
set @tempdescription = 'If its a service migrated document edit'
set @relatedStepDescription = 'If its an insurance migrated document edit'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a service migrated document edit'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempServiceValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details - Date of service'
set @tempdescription = 'If claimant modifies the migrated DOC details - Date of service'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details - Date of service'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Date of service_validate'
set @tempcondition = 2
set @tempvalue1 = '0d989b51-3bf4-482a-974e-d95d06c62db6'
set @tempvalue2 = ''
set @criteriaFieldID = '3715c351-2d0f-4b94-b65f-f9ec74cbc529'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Date of service'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Date of service to duplicate attribute'
set @tempdescription = 'Copy Date of service to duplicate attribute'
set @formula = '("[0d989b51-3bf4-482a-974e-d95d06c62db6]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Date of service_validate'
set @criteriaFieldID = '3715c351-2d0f-4b94-b65f-f9ec74cbc529'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Date of service'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details - Serviced by'
set @tempdescription = 'If claimant modifies the migrated DOC details - Serviced by'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details - Serviced by'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Serviced by_validate'
set @tempcondition = 2
set @tempvalue1 = 'cede4f51-9369-48ab-95c3-944ed81d4e17'
set @tempvalue2 = ''
set @criteriaFieldID = 'ae1b02bc-32cb-44cb-a5c4-0a81a8ca76a6'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Serviced by'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Serviced by to duplicate attribute'
set @tempdescription = 'Copy Serviced by to duplicate attribute'
set @formula = '("[cede4f51-9369-48ab-95c3-944ed81d4e17]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Serviced by_validate'
set @criteriaFieldID = 'ae1b02bc-32cb-44cb-a5c4-0a81a8ca76a6'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Serviced by'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details of vehicle - Document in service records'
set @tempdescription = 'If claimant modifies the migrated DOC details of vehicle - Document in service records'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details of vehicle - Document in service records'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @tempcondition = 2
set @tempvalue1 = '3e9ed916-08c9-4192-ad29-b2103b458d26'
set @tempvalue2 = ''
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details of vehicle - Document in service records'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Document to duplicate attribute'
set @tempdescription = 'Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details of vehicle - Document in service records'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if reviewer is null - data migration'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a tax migrated document edit'
set @tempdescription = 'If its a tax migrated document edit'
set @relatedStepDescription = 'If its an insurance migrated document edit'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a tax migrated document edit'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempTaxValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details - Sorn'
set @tempdescription = 'If claimant modifies the migrated DOC details - Sorn'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details - Sorn'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'SORN_validate'
set @tempcondition = 2
set @tempvalue1 = 'ec938d48-89d2-40f1-a06c-8c5602ca5dad'
set @tempvalue2 = ''
set @criteriaFieldID = 'f29ccb04-6214-48f3-b1c7-696c90319f51'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Sorn'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy SORN to duplicate attribute'
set @tempdescription = 'Copy SORN to duplicate attribute'
set @formula = '("[ec938d48-89d2-40f1-a06c-8c5602ca5dad]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'SORN_validate'
set @criteriaFieldID = 'f29ccb04-6214-48f3-b1c7-696c90319f51'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Sorn'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details - Checked by vehicle owner'
set @tempdescription = 'If claimant modifies the migrated DOC details - Checked by vehicle owner'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details - Checked by vehicle owner'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Checked by vehicle owner_validate'
set @tempcondition = 2
set @tempvalue1 = 'ee6447bd-b4ad-4b9a-a34f-b365aab9e369'
set @tempvalue2 = ''
set @criteriaFieldID = '44bae7eb-4f77-4fd0-83d7-38440526a7f0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Checked by vehicle owner'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Checked by vehicle owner to duplicate attribute'
set @tempdescription = 'Copy Checked by vehicle owner to duplicate attribute'
set @formula = '("[ee6447bd-b4ad-4b9a-a34f-b365aab9e369]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Checked by vehicle owner_validate'
set @criteriaFieldID = '44bae7eb-4f77-4fd0-83d7-38440526a7f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Checked by vehicle owner'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details - Document'
set @tempdescription = 'If claimant modifies the migrated DOC details - Document'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details - Document'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @tempcondition = 2
set @tempvalue1 = '3e9ed916-08c9-4192-ad29-b2103b458d26'
set @tempvalue2 = ''
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Document to duplicate attribute'
set @tempdescription = 'Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if reviewer is null - data migration'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a breakdown cover migrated document edit'
set @tempdescription = 'If its a breakdown cover migrated document edit'
set @relatedStepDescription = 'If its an insurance migrated document edit'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a breakdown cover migrated document edit'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempBreakDownValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details of vehicle - Document'
set @tempdescription = 'If claimant modifies the migrated DOC details of vehicle - Document'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details of vehicle - Document'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @tempcondition = 2
set @tempvalue1 = '3e9ed916-08c9-4192-ad29-b2103b458d26'
set @tempvalue2 = ''
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details of vehicle - Document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Document to duplicate attribute'
set @tempdescription = 'Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details of vehicle - Document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details - Provider in BreakDown'
set @tempdescription = 'If claimant modifies the migrated DOC details - Provider in BreakDown'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details - Provider in BreakDown'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Provider_validate'
set @tempcondition = 2
set @tempvalue1 = '27109f54-4305-4b64-896c-94c6a15b52c0'
set @tempvalue2 = ''
set @criteriaFieldID = 'a5e78abe-a398-473e-8a63-f734d9084e7e'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Provider in BreakDown'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Provider to duplicate attribute in BreakDown'
set @tempdescription = 'Copy Provider to duplicate attribute in BreakDown'
set @formula = '("[27109f54-4305-4b64-896c-94c6a15b52c0]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Provider_validate'
set @criteriaFieldID = 'a5e78abe-a398-473e-8a63-f734d9084e7e'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Provider in BreakDown'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover migrated document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details - Checked by vehicle owner in BreakDown Cover'
set @tempdescription = 'If claimant modifies the migrated DOC details - Checked by vehicle owner in BreakDown Cover'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details - Checked by vehicle owner in BreakDown Cover'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Checked by vehicle owner_validate'
set @tempcondition = 2
set @tempvalue1 = 'ee6447bd-b4ad-4b9a-a34f-b365aab9e369'
set @tempvalue2 = ''
set @criteriaFieldID = '44bae7eb-4f77-4fd0-83d7-38440526a7f0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Checked by vehicle owner in BreakDown Cover'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Checked by vehicle owner to duplicate attribute'
set @tempdescription = 'Copy Checked by vehicle owner to duplicate attribute'
set @formula = '("[ee6447bd-b4ad-4b9a-a34f-b365aab9e369]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Checked by vehicle owner_validate'
set @criteriaFieldID = '44bae7eb-4f77-4fd0-83d7-38440526a7f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details - Checked by vehicle owner in BreakDown Cover'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if reviewer is null - data migration'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details of vehicle - vehicle'
set @tempdescription = 'If claimant modifies the migrated DOC details of vehicle - vehicle'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details of vehicle - vehicle'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Vehicle_Validate'
set @tempcondition = 2
set @tempvalue1 = 'efbd0c7f-0ecc-406f-be6c-ed9978604439'
set @tempvalue2 = ''
set @criteriaFieldID = '39d4b180-0a49-4edc-9ee7-834f4a7c04af'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details of vehicle - vehicle'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy vehicle id to duplicate attribute'
set @tempdescription = 'Copy vehicle id to duplicate attribute'
set @formula = '("[efbd0c7f-0ecc-406f-be6c-ed9978604439]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Vehicle_Validate'
set @criteriaFieldID = '39d4b180-0a49-4edc-9ee7-834f4a7c04af'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details of vehicle - vehicle'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if reviewer is null - data migration'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details of vehicle - Start date'
set @tempdescription = 'If claimant modifies the migrated DOC details of vehicle - Start date'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details of vehicle - Start date'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Start date_validate'
set @tempcondition = 2
set @tempvalue1 = '732bd235-49b5-41d0-8420-6534ebe23ed8'
set @tempvalue2 = ''
set @criteriaFieldID = 'b0987174-1e87-4b62-8a85-dfa6c9366557'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details of vehicle - Start date'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Start date to duplicate attribute'
set @tempdescription = 'Copy Start date to duplicate attribute'
set @formula = '("[732bd235-49b5-41d0-8420-6534ebe23ed8]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Start date_validate'
set @criteriaFieldID = 'b0987174-1e87-4b62-8a85-dfa6c9366557'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details of vehicle - Start date'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if reviewer is null - data migration'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details of vehicle - Expiry date'
set @tempdescription = 'If claimant modifies the migrated DOC details of vehicle - Expiry date'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details of vehicle - Expiry date'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Expiry date_validate'
set @tempcondition = 2
set @tempvalue1 = '9f6e275f-516e-4a71-a94c-0d3d26a77d38'
set @tempvalue2 = ''
set @criteriaFieldID = '3ba3a3ec-d6cb-443d-b77c-14672e7af9f0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details of vehicle - Expiry date'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Expiry date to duplicate attribute'
set @tempdescription = 'Copy Expiry date to duplicate attribute'
set @formula = '("[9f6e275f-516e-4a71-a94c-0d3d26a77d38]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Expiry date_validate'
set @criteriaFieldID = '3ba3a3ec-d6cb-443d-b77c-14672e7af9f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details of vehicle - Expiry date'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if reviewer is null - data migration'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the migrated DOC details'
set @tempdescription = 'If claimant modifies the migrated DOC details'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the migrated DOC details'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @tempcondition = 1
set @tempvalue1 = @tempInvalidatedValue
set @tempvalue2 = ''
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Invalidated review'
set @tempdescription = 'Invalidated review'
set @tempemailtemplatename = 'Send to an approver when claimant modifies a vehicle document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the migrated DOC details'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if reviewer is null - data migration'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow After migrated Values are assigned'
set @tempdescription = 'Finish Workflow After migrated Values are assigned'
set @relatedStepDescription = 'If claimant modifies the migrated DOC details'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 10, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Finish Workflow After migrated Values are assigned'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @parentdescription = 'Finish Workflow After migrated Values are assigned'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the review is good'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Check if the user is reviewer whilst uploading new doc'
set @tempdescription = 'Check if the user is reviewer whilst uploading new doc'
set @relatedStepDescription = 'Check if reviewer is null - data migration'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Check if the user is reviewer whilst uploading new doc'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'ApproverCheck'
set @tempcondition = 1
set @tempvalue1 = '0'
set @tempvalue2 = ''
set @criteriaFieldID = 'f5ace912-2c70-4e56-bf56-62a05e0f62ff'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Set the Review Date'
set @tempdescription = 'Reviewer - Set the Review Date'
set @formula = 'TEXT(NOW(),"yyyy-MM-dd HH:mm:ss.fff")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Review date'
set @criteriaFieldID = 'b1ee967d-cea8-4db6-8410-9992478b5be7'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy vehicle id to duplicate attribute'
set @tempdescription = 'Reviewer - Copy vehicle id to duplicate attribute'
set @formula = '("[efbd0c7f-0ecc-406f-be6c-ed9978604439]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Vehicle_Validate'
set @criteriaFieldID = '39d4b180-0a49-4edc-9ee7-834f4a7c04af'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Start date to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Start date to duplicate attribute'
set @formula = '("[732bd235-49b5-41d0-8420-6534ebe23ed8]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Start date_validate'
set @criteriaFieldID = 'b0987174-1e87-4b62-8a85-dfa6c9366557'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Expiry date to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Expiry date to duplicate attribute'
set @formula = '("[9f6e275f-516e-4a71-a94c-0d3d26a77d38]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Expiry date_validate'
set @criteriaFieldID = '3ba3a3ec-d6cb-443d-b77c-14672e7af9f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its an insurance document edit by approver - inner check'
set @tempdescription = 'If its an insurance document edit by approver - inner check'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its an insurance document edit by approver - inner check'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempInsuranceValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Document to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Is commuter travel included? to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Is commuter travel included? to duplicate attribute'
set @formula = '("[99bfcf34-5a0d-4efc-9c06-e6d2c87ddc4c]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Is commuter travel included_validate'
set @criteriaFieldID = 'a315feb5-efe7-4bfe-9d01-b2d941c716a5'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Is class 1 business travel included? to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Is class 1 business travel included? to duplicate attribute'
set @formula = '("[b2fa95dd-85df-4e8b-a4d0-2ae46abcc7ca]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Is Class 1 check_validate'
set @criteriaFieldID = 'bad8a1b7-ec39-4793-af86-cc5c859f2953'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy policy number to duplicate attribute'
set @tempdescription = 'Reviewer - Copy policy number to duplicate attribute'
set @formula = '("[afff4d09-7350-4262-b0f9-ae25de53b7d5]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Policy number_validate'
set @criteriaFieldID = '27f0f5bf-1071-433f-bbe4-b6f9b4111eea'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Cover type to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Cover type to duplicate attribute'
set @formula = '("[5775929a-f3ac-42e0-be9d-c47fbd430d01]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Cover type_validate'
set @criteriaFieldID = 'ef872f14-88bf-4c0c-bdee-47d9a2c539d9'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Provider to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Provider to duplicate attribute'
set @formula = '("[27109f54-4305-4b64-896c-94c6a15b52c0]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Provider_validate'
set @criteriaFieldID = 'a5e78abe-a398-473e-8a63-f734d9084e7e'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a MOT document edit by approver - inner check'
set @tempdescription = 'If its a MOT document edit by approver - inner check'
set @relatedStepDescription = 'If its an insurance document edit by approver - inner check'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a MOT document edit by approver - inner check'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempMOTValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Document to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy test number to duplicate attribute'
set @tempdescription = 'Reviewer - Copy test number to duplicate attribute'
set @formula = '("[a0a992e3-2e76-4260-b111-5eff4366ae35]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Test number_validate'
set @criteriaFieldID = 'ca355e81-30ab-4e51-adc4-9b723e26fe5a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Do you require an MOT? to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Do you require an MOT? to duplicate attribute'
set @formula = '("[f430ae0c-9a46-49c8-8f91-068db4b33c9b]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Do you require an MOT_validate'
set @criteriaFieldID = '8f24a33e-5ce8-4545-9237-e4a9b6697842'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a service document edit by approver - inner check'
set @tempdescription = 'If its a service document edit by approver - inner check'
set @relatedStepDescription = 'If its an insurance document edit by approver - inner check'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a service document edit by approver - inner check'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempServiceValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Document to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Date of service to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Date of service to duplicate attribute'
set @formula = '("[0d989b51-3bf4-482a-974e-d95d06c62db6]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Date of service_validate'
set @criteriaFieldID = '3715c351-2d0f-4b94-b65f-f9ec74cbc529'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Serviced by to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Serviced by to duplicate attribute'
set @formula = '("[cede4f51-9369-48ab-95c3-944ed81d4e17]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Serviced by_validate'
set @criteriaFieldID = 'ae1b02bc-32cb-44cb-a5c4-0a81a8ca76a6'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a tax document edit by approver - inner check'
set @tempdescription = 'If its a tax document edit by approver - inner check'
set @relatedStepDescription = 'If its an insurance document edit by approver - inner check'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a tax document edit by approver - inner check'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempTaxValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Checked by vehicle owner to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Checked by vehicle owner to duplicate attribute'
set @formula = '("[ee6447bd-b4ad-4b9a-a34f-b365aab9e369]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Checked by vehicle owner_validate'
set @criteriaFieldID = '44bae7eb-4f77-4fd0-83d7-38440526a7f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy SORN to duplicate attribute'
set @tempdescription = 'Reviewer - Copy SORN to duplicate attribute'
set @formula = '("[ec938d48-89d2-40f1-a06c-8c5602ca5dad]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'SORN_validate'
set @criteriaFieldID = 'f29ccb04-6214-48f3-b1c7-696c90319f51'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Document to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a breakdown cover document edit by approver - inner check'
set @tempdescription = 'If its a breakdown cover document edit by approver - inner check'
set @relatedStepDescription = 'If its an insurance document edit by approver - inner check'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a breakdown cover document edit by approver - inner check'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempBreakDownValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Checked by vehicle owner to duplicate attribute for Breakdown Cover'
set @tempdescription = 'Reviewer - Copy Checked by vehicle owner to duplicate attribute for Breakdown Cover'
set @formula = '("[ee6447bd-b4ad-4b9a-a34f-b365aab9e369]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Checked by vehicle owner_validate'
set @criteriaFieldID = '44bae7eb-4f77-4fd0-83d7-38440526a7f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Provider to duplicate attribute for BreakDown Cover'
set @tempdescription = 'Reviewer - Copy Provider to duplicate attribute for BreakDown Cover'
set @formula = '("[27109f54-4305-4b64-896c-94c6a15b52c0]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Provider_validate'
set @criteriaFieldID = 'a5e78abe-a398-473e-8a63-f734d9084e7e'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document edit by approver - inner check'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Document to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the review is good'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Check if the user is reviewer whilst the review is passed'
set @tempdescription = 'Check if the user is reviewer whilst the review is passed'
set @relatedStepDescription = 'Check if reviewer is null - data migration'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Check if the user is reviewer whilst the review is passed'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Reviewed by'
set @tempcondition = 1
set @tempvalue1 = 'deb645a2-1a9d-48e2-8801-46e7a69802ac'
set @tempvalue2 = ''
set @criteriaFieldID = '80591948-5246-4d25-b606-c99a93d5f911'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Set the Review Date'
set @tempdescription = 'Reviewer - Set the Review Date'
set @formula = 'TEXT(NOW(),"yyyy-MM-dd HH:mm:ss.fff")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Review date'
set @criteriaFieldID = 'b1ee967d-cea8-4db6-8410-9992478b5be7'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy vehicle id to duplicate attribute'
set @tempdescription = 'Reviewer - Copy vehicle id to duplicate attribute'
set @formula = '("[efbd0c7f-0ecc-406f-be6c-ed9978604439]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Vehicle_Validate'
set @criteriaFieldID = '39d4b180-0a49-4edc-9ee7-834f4a7c04af'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Start date to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Start date to duplicate attribute'
set @formula = '("[732bd235-49b5-41d0-8420-6534ebe23ed8]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Start date_validate'
set @criteriaFieldID = 'b0987174-1e87-4b62-8a85-dfa6c9366557'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Expiry date to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Expiry date to duplicate attribute'
set @formula = '("[9f6e275f-516e-4a71-a94c-0d3d26a77d38]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Expiry date_validate'
set @criteriaFieldID = '3ba3a3ec-d6cb-443d-b77c-14672e7af9f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its an insurance document edit by approver'
set @tempdescription = 'If its an insurance document edit by approver'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its an insurance document edit by approver'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempInsuranceValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Document to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Is commuter travel included? to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Is commuter travel included? to duplicate attribute'
set @formula = '("[99bfcf34-5a0d-4efc-9c06-e6d2c87ddc4c]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Is commuter travel included_validate'
set @criteriaFieldID = 'a315feb5-efe7-4bfe-9d01-b2d941c716a5'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Is class 1 business travel included? to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Is class 1 business travel included? to duplicate attribute'
set @formula = '("[b2fa95dd-85df-4e8b-a4d0-2ae46abcc7ca]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Is Class 1 check_validate'
set @criteriaFieldID = 'bad8a1b7-ec39-4793-af86-cc5c859f2953'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy policy number to duplicate attribute'
set @tempdescription = 'Reviewer - Copy policy number to duplicate attribute'
set @formula = '("[afff4d09-7350-4262-b0f9-ae25de53b7d5]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Policy number_validate'
set @criteriaFieldID = '27f0f5bf-1071-433f-bbe4-b6f9b4111eea'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Cover type to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Cover type to duplicate attribute'
set @formula = '("[5775929a-f3ac-42e0-be9d-c47fbd430d01]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Cover type_validate'
set @criteriaFieldID = 'ef872f14-88bf-4c0c-bdee-47d9a2c539d9'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Provider to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Provider to duplicate attribute'
set @formula = '("[27109f54-4305-4b64-896c-94c6a15b52c0]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Provider_validate'
set @criteriaFieldID = 'a5e78abe-a398-473e-8a63-f734d9084e7e'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a MOT document edit by approver'
set @tempdescription = 'If its a MOT document edit by approver'
set @relatedStepDescription = 'If its an insurance document edit by approver'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a MOT document edit by approver'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempMOTValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Document to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy test number to duplicate attribute'
set @tempdescription = 'Reviewer - Copy test number to duplicate attribute'
set @formula = '("[a0a992e3-2e76-4260-b111-5eff4366ae35]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Test number_validate'
set @criteriaFieldID = 'ca355e81-30ab-4e51-adc4-9b723e26fe5a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Do you require an MOT? to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Do you require an MOT? to duplicate attribute'
set @formula = '("[f430ae0c-9a46-49c8-8f91-068db4b33c9b]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Do you require an MOT_validate'
set @criteriaFieldID = '8f24a33e-5ce8-4545-9237-e4a9b6697842'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a service document edit by approver'
set @tempdescription = 'If its a service document edit by approver'
set @relatedStepDescription = 'If its an insurance document edit by approver'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a service document edit by approver'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempServiceValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Document to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Date of service to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Date of service to duplicate attribute'
set @formula = '("[0d989b51-3bf4-482a-974e-d95d06c62db6]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Date of service_validate'
set @criteriaFieldID = '3715c351-2d0f-4b94-b65f-f9ec74cbc529'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Serviced by to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Serviced by to duplicate attribute'
set @formula = '("[cede4f51-9369-48ab-95c3-944ed81d4e17]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Serviced by_validate'
set @criteriaFieldID = 'ae1b02bc-32cb-44cb-a5c4-0a81a8ca76a6'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a tax document edit by approver'
set @tempdescription = 'If its a tax document edit by approver'
set @relatedStepDescription = 'If its an insurance document edit by approver'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a tax document edit by approver'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempTaxValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Checked by vehicle owner to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Checked by vehicle owner to duplicate attribute'
set @formula = '("[ee6447bd-b4ad-4b9a-a34f-b365aab9e369]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Checked by vehicle owner_validate'
set @criteriaFieldID = '44bae7eb-4f77-4fd0-83d7-38440526a7f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy SORN to duplicate attribute'
set @tempdescription = 'Reviewer - Copy SORN to duplicate attribute'
set @formula = '("[ec938d48-89d2-40f1-a06c-8c5602ca5dad]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'SORN_validate'
set @criteriaFieldID = 'f29ccb04-6214-48f3-b1c7-696c90319f51'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Document to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a breakdown cover document edit by approver'
set @tempdescription = 'If its a breakdown cover document edit by approver'
set @relatedStepDescription = 'If its an insurance document edit by approver'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a breakdown cover document edit by approver'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempBreakDownValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Checked by vehicle owner to duplicate attribute for Breakdown Cover'
set @tempdescription = 'Reviewer - Copy Checked by vehicle owner to duplicate attribute for Breakdown Cover'
set @formula = '("[ee6447bd-b4ad-4b9a-a34f-b365aab9e369]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Checked by vehicle owner_validate'
set @criteriaFieldID = '44bae7eb-4f77-4fd0-83d7-38440526a7f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Provider to duplicate attribute for BreakDown Cover'
set @tempdescription = 'Reviewer - Copy Provider to duplicate attribute for BreakDown Cover'
set @formula = '("[27109f54-4305-4b64-896c-94c6a15b52c0]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Provider_validate'
set @criteriaFieldID = 'a5e78abe-a398-473e-8a63-f734d9084e7e'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document edit by approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Reviewer - Copy Document to duplicate attribute'
set @tempdescription = 'Reviewer - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)

print 'Creating workflow step Send Email to Notify claimant if the approver has approved the insurance document'
set @tempdescription = 'Send Email to Notify claimant if the approver has approved the insurance document'
set @tempemailtemplatename = 'Send to the claimant if the approver has approved the vehicle document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)

print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the review is good'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If the user is claimant'
set @tempdescription = 'If the user is claimant'
set @relatedStepDescription = 'Check if reviewer is null - data migration'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If the user is claimant'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Reviewed by'
set @tempcondition = 2
set @tempvalue1 = 'deb645a2-1a9d-48e2-8801-46e7a69802ac'
set @tempvalue2 = ''
set @criteriaFieldID = '80591948-5246-4d25-b606-c99a93d5f911'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its an insurance document edit'
set @tempdescription = 'If its an insurance document edit'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its an insurance document edit'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempInsuranceValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Policy number'
set @tempdescription = 'If claimant modifies the DOC details - Policy number'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Policy number'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Policy number_validate'
set @tempcondition = 2
set @tempvalue1 = 'afff4d09-7350-4262-b0f9-ae25de53b7d5'
set @tempvalue2 = ''
set @criteriaFieldID = '27f0f5bf-1071-433f-bbe4-b6f9b4111eea'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Policy number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy policy number to duplicate attribute'
set @tempdescription = 'Copy policy number to duplicate attribute'
set @formula = '("[afff4d09-7350-4262-b0f9-ae25de53b7d5]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Policy number_validate'
set @criteriaFieldID = '27f0f5bf-1071-433f-bbe4-b6f9b4111eea'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Policy number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Is commuter travel included?'
set @tempdescription = 'If claimant modifies the DOC details - Is commuter travel included?'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Is commuter travel included?'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Is commuter travel included_validate'
set @tempcondition = 2
set @tempvalue1 = '99bfcf34-5a0d-4efc-9c06-e6d2c87ddc4c'
set @tempvalue2 = ''
set @criteriaFieldID = 'a315feb5-efe7-4bfe-9d01-b2d941c716a5'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Is commuter travel included?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Is commuter travel included? to duplicate attribute'
set @tempdescription = 'Copy Is commuter travel included? to duplicate attribute'
set @formula = '("[99bfcf34-5a0d-4efc-9c06-e6d2c87ddc4c]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Is commuter travel included_validate'
set @criteriaFieldID = 'a315feb5-efe7-4bfe-9d01-b2d941c716a5'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Is commuter travel included?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Is class 1 business travel included?'
set @tempdescription = 'If claimant modifies the DOC details - Is class 1 business travel included?'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Is class 1 business travel included?'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Is class 1 check_validate'
set @tempcondition = 2
set @tempvalue1 = 'b2fa95dd-85df-4e8b-a4d0-2ae46abcc7ca'
set @tempvalue2 = ''
set @criteriaFieldID = 'bad8a1b7-ec39-4793-af86-cc5c859f2953'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Is class 1 business travel included?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Is class 1 business travel included? to duplicate attribute'
set @tempdescription = 'Copy Is class 1 business travel included? to duplicate attribute'
set @formula = '("[b2fa95dd-85df-4e8b-a4d0-2ae46abcc7ca]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Is Class 1 check_validate'
set @criteriaFieldID = 'bad8a1b7-ec39-4793-af86-cc5c859f2953'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Is class 1 business travel included?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Cover type'
set @tempdescription = 'If claimant modifies the DOC details - Cover type'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Cover type'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Cover type_validate'
set @tempcondition = 2
set @tempvalue1 = '5775929a-f3ac-42e0-be9d-c47fbd430d01'
set @tempvalue2 = ''
set @criteriaFieldID = 'ef872f14-88bf-4c0c-bdee-47d9a2c539d9'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Cover type'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Cover type to duplicate attribute'
set @tempdescription = 'Copy Cover type to duplicate attribute'
set @formula = '("[5775929a-f3ac-42e0-be9d-c47fbd430d01]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Cover type_validate'
set @criteriaFieldID = 'ef872f14-88bf-4c0c-bdee-47d9a2c539d9'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Cover type'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Provider'
set @tempdescription = 'If claimant modifies the DOC details - Provider'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Provider'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Provider_validate'
set @tempcondition = 2
set @tempvalue1 = '27109f54-4305-4b64-896c-94c6a15b52c0'
set @tempvalue2 = ''
set @criteriaFieldID = 'a5e78abe-a398-473e-8a63-f734d9084e7e'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Provider'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Provider to duplicate attribute'
set @tempdescription = 'Copy Provider to duplicate attribute'
set @formula = '("[27109f54-4305-4b64-896c-94c6a15b52c0]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Provider_validate'
set @criteriaFieldID = 'a5e78abe-a398-473e-8a63-f734d9084e7e'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Provider'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details of vehicle - Document in Insurance'
set @tempdescription = 'If claimant modifies the DOC details of vehicle - Document in Insurance'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details of vehicle - Document in Insurance'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @tempcondition = 2
set @tempvalue1 = '3e9ed916-08c9-4192-ad29-b2103b458d26'
set @tempvalue2 = ''
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details of vehicle - Document in Insurance'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Document to duplicate attribute'
set @tempdescription = 'Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details of vehicle - Document in Insurance'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a MOT document edit'
set @tempdescription = 'If its a MOT document edit'
set @relatedStepDescription = 'If its an insurance document edit'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a MOT document edit'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempMOTValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Do you require an MOT?'
set @tempdescription = 'If claimant modifies the DOC details - Do you require an MOT?'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Do you require an MOT?'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Do you require an MOT_validate'
set @tempcondition = 2
set @tempvalue1 = 'f430ae0c-9a46-49c8-8f91-068db4b33c9b'
set @tempvalue2 = ''
set @criteriaFieldID = '8f24a33e-5ce8-4545-9237-e4a9b6697842'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Do you require an MOT?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Do you require an MOT? to duplicate attribute'
set @tempdescription = 'Copy Do you require an MOT? to duplicate attribute'
set @formula = '("[f430ae0c-9a46-49c8-8f91-068db4b33c9b]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Do you require an MOT_validate'
set @criteriaFieldID = '8f24a33e-5ce8-4545-9237-e4a9b6697842'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Do you require an MOT?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - test number'
set @tempdescription = 'If claimant modifies the DOC details - test number'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - test number'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Test number_validate'
set @tempcondition = 2
set @tempvalue1 = 'a0a992e3-2e76-4260-b111-5eff4366ae35'
set @tempvalue2 = ''
set @criteriaFieldID = 'ca355e81-30ab-4e51-adc4-9b723e26fe5a'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - test number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy test number to duplicate attribute'
set @tempdescription = 'Copy test number to duplicate attribute'
set @formula = '("[a0a992e3-2e76-4260-b111-5eff4366ae35]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Test number_validate'
set @criteriaFieldID = 'ca355e81-30ab-4e51-adc4-9b723e26fe5a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - test number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details of vehicle - Document in MOT records'
set @tempdescription = 'If claimant modifies the DOC details of vehicle - Document in MOT records'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details of vehicle - Document in MOT records'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @tempcondition = 2
set @tempvalue1 = '3e9ed916-08c9-4192-ad29-b2103b458d26'
set @tempvalue2 = ''
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details of vehicle - Document in MOT records'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Document to duplicate attribute'
set @tempdescription = 'Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details of vehicle - Document in MOT records'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a service document edit'
set @tempdescription = 'If its a service document edit'
set @relatedStepDescription = 'If its an insurance document edit'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a service document edit'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempServiceValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Date of service'
set @tempdescription = 'If claimant modifies the DOC details - Date of service'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Date of service'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Date of service_validate'
set @tempcondition = 2
set @tempvalue1 = '0d989b51-3bf4-482a-974e-d95d06c62db6'
set @tempvalue2 = ''
set @criteriaFieldID = '3715c351-2d0f-4b94-b65f-f9ec74cbc529'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Date of service'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Date of service to duplicate attribute'
set @tempdescription = 'Copy Date of service to duplicate attribute'
set @formula = '("[0d989b51-3bf4-482a-974e-d95d06c62db6]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Date of service_validate'
set @criteriaFieldID = '3715c351-2d0f-4b94-b65f-f9ec74cbc529'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Date of service'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Serviced by'
set @tempdescription = 'If claimant modifies the DOC details - Serviced by'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Serviced by'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Serviced by_validate'
set @tempcondition = 2
set @tempvalue1 = 'cede4f51-9369-48ab-95c3-944ed81d4e17'
set @tempvalue2 = ''
set @criteriaFieldID = 'ae1b02bc-32cb-44cb-a5c4-0a81a8ca76a6'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Serviced by'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Serviced by to duplicate attribute'
set @tempdescription = 'Copy Serviced by to duplicate attribute'
set @formula = '("[cede4f51-9369-48ab-95c3-944ed81d4e17]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Serviced by_validate'
set @criteriaFieldID = 'ae1b02bc-32cb-44cb-a5c4-0a81a8ca76a6'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Serviced by'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details of vehicle - Document in service records'
set @tempdescription = 'If claimant modifies the DOC details of vehicle - Document in service records'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details of vehicle - Document in service records'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @tempcondition = 2
set @tempvalue1 = '3e9ed916-08c9-4192-ad29-b2103b458d26'
set @tempvalue2 = ''
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details of vehicle - Document in service records'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Document to duplicate attribute'
set @tempdescription = 'Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details of vehicle - Document in service records'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a tax document edit'
set @tempdescription = 'If its a tax document edit'
set @relatedStepDescription = 'If its an insurance document edit'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a tax document edit'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempTaxValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Sorn'
set @tempdescription = 'If claimant modifies the DOC details - Sorn'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Sorn'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'SORN_validate'
set @tempcondition = 2
set @tempvalue1 = 'ec938d48-89d2-40f1-a06c-8c5602ca5dad'
set @tempvalue2 = ''
set @criteriaFieldID = 'f29ccb04-6214-48f3-b1c7-696c90319f51'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Sorn'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy SORN to duplicate attribute'
set @tempdescription = 'Copy SORN to duplicate attribute'
set @formula = '("[ec938d48-89d2-40f1-a06c-8c5602ca5dad]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'SORN_validate'
set @criteriaFieldID = 'f29ccb04-6214-48f3-b1c7-696c90319f51'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Sorn'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Checked by vehicle owner'
set @tempdescription = 'If claimant modifies the DOC details - Checked by vehicle owner'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Checked by vehicle owner'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Checked by vehicle owner_validate'
set @tempcondition = 2
set @tempvalue1 = 'ee6447bd-b4ad-4b9a-a34f-b365aab9e369'
set @tempvalue2 = ''
set @criteriaFieldID = '44bae7eb-4f77-4fd0-83d7-38440526a7f0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Checked by vehicle owner'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Checked by vehicle owner to duplicate attribute'
set @tempdescription = 'Copy Checked by vehicle owner to duplicate attribute'
set @formula = '("[ee6447bd-b4ad-4b9a-a34f-b365aab9e369]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Checked by vehicle owner_validate'
set @criteriaFieldID = '44bae7eb-4f77-4fd0-83d7-38440526a7f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Checked by vehicle owner'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Document'
set @tempdescription = 'If claimant modifies the DOC details - Document'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Document'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @tempcondition = 2
set @tempvalue1 = '3e9ed916-08c9-4192-ad29-b2103b458d26'
set @tempvalue2 = ''
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Document to duplicate attribute'
set @tempdescription = 'Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a breakdown cover document edit'
set @tempdescription = 'If its a breakdown cover document edit'
set @relatedStepDescription = 'If its an insurance document edit'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a breakdown cover document edit'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempBreakDownValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details of vehicle - Document'
set @tempdescription = 'If claimant modifies the DOC details of vehicle - Document'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details of vehicle - Document'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @tempcondition = 2
set @tempvalue1 = '3e9ed916-08c9-4192-ad29-b2103b458d26'
set @tempvalue2 = ''
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details of vehicle - Document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Document to duplicate attribute'
set @tempdescription = 'Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details of vehicle - Document'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Provider in BreakDown'
set @tempdescription = 'If claimant modifies the DOC details - Provider in BreakDown'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Provider in BreakDown'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Provider_validate'
set @tempcondition = 2
set @tempvalue1 = '27109f54-4305-4b64-896c-94c6a15b52c0'
set @tempvalue2 = ''
set @criteriaFieldID = 'a5e78abe-a398-473e-8a63-f734d9084e7e'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Provider in BreakDown'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Provider to duplicate attribute in BreakDown'
set @tempdescription = 'Copy Provider to duplicate attribute in BreakDown'
set @formula = '("[27109f54-4305-4b64-896c-94c6a15b52c0]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Provider_validate'
set @criteriaFieldID = 'a5e78abe-a398-473e-8a63-f734d9084e7e'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Provider in BreakDown'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Checked by vehicle owner in BreakDown Cover'
set @tempdescription = 'If claimant modifies the DOC details - Checked by vehicle owner in BreakDown Cover'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Checked by vehicle owner in BreakDown Cover'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Checked by vehicle owner_validate'
set @tempcondition = 2
set @tempvalue1 = 'ee6447bd-b4ad-4b9a-a34f-b365aab9e369'
set @tempvalue2 = ''
set @criteriaFieldID = '44bae7eb-4f77-4fd0-83d7-38440526a7f0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Checked by vehicle owner in BreakDown Cover'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Checked by vehicle owner to duplicate attribute'
set @tempdescription = 'Copy Checked by vehicle owner to duplicate attribute'
set @formula = '("[ee6447bd-b4ad-4b9a-a34f-b365aab9e369]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Checked by vehicle owner_validate'
set @criteriaFieldID = '44bae7eb-4f77-4fd0-83d7-38440526a7f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Checked by vehicle owner in BreakDown Cover'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details of vehicle - vehicle'
set @tempdescription = 'If claimant modifies the DOC details of vehicle - vehicle'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details of vehicle - vehicle'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Vehicle_Validate'
set @tempcondition = 2
set @tempvalue1 = 'efbd0c7f-0ecc-406f-be6c-ed9978604439'
set @tempvalue2 = ''
set @criteriaFieldID = '39d4b180-0a49-4edc-9ee7-834f4a7c04af'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details of vehicle - vehicle'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy vehicle id to duplicate attribute'
set @tempdescription = 'Copy vehicle id to duplicate attribute'
set @formula = '("[efbd0c7f-0ecc-406f-be6c-ed9978604439]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Vehicle_Validate'
set @criteriaFieldID = '39d4b180-0a49-4edc-9ee7-834f4a7c04af'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details of vehicle - vehicle'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details of vehicle - Start date'
set @tempdescription = 'If claimant modifies the DOC details of vehicle - Start date'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details of vehicle - Start date'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Start date_validate'
set @tempcondition = 2
set @tempvalue1 = '732bd235-49b5-41d0-8420-6534ebe23ed8'
set @tempvalue2 = ''
set @criteriaFieldID = 'b0987174-1e87-4b62-8a85-dfa6c9366557'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details of vehicle - Start date'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Start date to duplicate attribute'
set @tempdescription = 'Copy Start date to duplicate attribute'
set @formula = '("[732bd235-49b5-41d0-8420-6534ebe23ed8]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Start date_validate'
set @criteriaFieldID = 'b0987174-1e87-4b62-8a85-dfa6c9366557'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details of vehicle - Start date'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details of vehicle - Expiry date'
set @tempdescription = 'If claimant modifies the DOC details of vehicle - Expiry date'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details of vehicle - Expiry date'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Expiry date_validate'
set @tempcondition = 2
set @tempvalue1 = '9f6e275f-516e-4a71-a94c-0d3d26a77d38'
set @tempvalue2 = ''
set @criteriaFieldID = '3ba3a3ec-d6cb-443d-b77c-14672e7af9f0'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details of vehicle - Expiry date'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Expiry date to duplicate attribute'
set @tempdescription = 'Copy Expiry date to duplicate attribute'
set @formula = '("[9f6e275f-516e-4a71-a94c-0d3d26a77d38]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Expiry date_validate'
set @criteriaFieldID = '3ba3a3ec-d6cb-443d-b77c-14672e7af9f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details of vehicle - Expiry date'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details'
set @tempdescription = 'If claimant modifies the DOC details'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @tempcondition = 1
set @tempvalue1 = @tempInvalidatedValue
set @tempvalue2 = ''
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Invalidated review'
set @tempdescription = 'Invalidated review'
set @tempemailtemplatename = 'Send to an approver when claimant modifies a vehicle document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow After Values are assigned'
set @tempdescription = 'Finish Workflow After Values are assigned'
set @relatedStepDescription = 'If claimant modifies the DOC details'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 10, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Finish Workflow After Values are assigned'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @parentdescription = 'Finish Workflow After Values are assigned'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @tempparentstepid = 0
print 'Creating workflow step If there is a review request from Claimant'
set @tempdescription = 'If there is a review request from Claimant'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 10, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If there is a review request from Claimant'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Assign Approver Check'
set @tempdescription = 'Assign Approver Check'
set @formula = 'IF("[80591948-5246-4d25-b606-c99a93d5f911]">" ",1,0)'
set @tablename = 'Vehicle Document'
set @fieldname = 'ApproverCheck'
set @criteriaFieldID = 'f5ace912-2c70-4e56-bf56-62a05e0f62ff'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy vehicle id to duplicate attribute'
set @tempdescription = 'New Request - Copy vehicle id to duplicate attribute'
set @formula = '("[efbd0c7f-0ecc-406f-be6c-ed9978604439]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Vehicle_Validate'
set @criteriaFieldID = '39d4b180-0a49-4edc-9ee7-834f4a7c04af'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Start date to duplicate attribute'
set @tempdescription = 'New Request - Copy Start date to duplicate attribute'
set @formula = '("[732bd235-49b5-41d0-8420-6534ebe23ed8]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Start date_validate'
set @criteriaFieldID = 'b0987174-1e87-4b62-8a85-dfa6c9366557'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Expiry date to duplicate attribute'
set @tempdescription = 'New Request - Copy Expiry date to duplicate attribute'
set @formula = '("[9f6e275f-516e-4a71-a94c-0d3d26a77d38]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Expiry date_validate'
set @criteriaFieldID = '3ba3a3ec-d6cb-443d-b77c-14672e7af9f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its an insurance document review'
set @tempdescription = 'If its an insurance document review'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its an insurance document review'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempInsuranceValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Is commuter travel included? to duplicate attribute'
set @tempdescription = 'New Request - Copy Is commuter travel included? to duplicate attribute'
set @formula = '("[99bfcf34-5a0d-4efc-9c06-e6d2c87ddc4c]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Is commuter travel included_validate'
set @criteriaFieldID = 'a315feb5-efe7-4bfe-9d01-b2d941c716a5'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Is class 1 business travel included? to duplicate attribute'
set @tempdescription = 'New Request - Copy Is class 1 business travel included? to duplicate attribute'
set @formula = '("[b2fa95dd-85df-4e8b-a4d0-2ae46abcc7ca]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Is Class 1 check_validate'
set @criteriaFieldID = 'bad8a1b7-ec39-4793-af86-cc5c859f2953'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy policy number to duplicate attribute'
set @tempdescription = 'New Request - Copy policy number to duplicate attribute'
set @formula = '("[afff4d09-7350-4262-b0f9-ae25de53b7d5]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Policy number_validate'
set @criteriaFieldID = '27f0f5bf-1071-433f-bbe4-b6f9b4111eea'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Cover type to duplicate attribute'
set @tempdescription = 'New Request - Copy Cover type to duplicate attribute'
set @formula = '("[5775929a-f3ac-42e0-be9d-c47fbd430d01]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Cover type_validate'
set @criteriaFieldID = 'ef872f14-88bf-4c0c-bdee-47d9a2c539d9'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Provider to duplicate attribute'
set @tempdescription = 'New Request - Copy Provider to duplicate attribute'
set @formula = '("[27109f54-4305-4b64-896c-94c6a15b52c0]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Provider_validate'
set @criteriaFieldID = 'a5e78abe-a398-473e-8a63-f734d9084e7e'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its an insurance document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Document to duplicate attribute'
set @tempdescription = 'New Request - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a MOT document review'
set @tempdescription = 'If its a MOT document review'
set @relatedStepDescription = 'If its an insurance document review'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a MOT document review'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempMOTValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy test number to duplicate attribute'
set @tempdescription = 'New Request - Copy test number to duplicate attribute'
set @formula = '("[a0a992e3-2e76-4260-b111-5eff4366ae35]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Test number_validate'
set @criteriaFieldID = 'ca355e81-30ab-4e51-adc4-9b723e26fe5a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Do you require an MOT? to duplicate attribute'
set @tempdescription = 'New Request - Copy Do you require an MOT? to duplicate attribute'
set @formula = '("[f430ae0c-9a46-49c8-8f91-068db4b33c9b]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Do you require an MOT_validate'
set @criteriaFieldID = '8f24a33e-5ce8-4545-9237-e4a9b6697842'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a MOT document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Document to duplicate attribute'
set @tempdescription = 'New Request - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a service document review'
set @tempdescription = 'If its a service document review'
set @relatedStepDescription = 'If its an insurance document review'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a service document review'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempServiceValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Date of service to duplicate attribute'
set @tempdescription = 'New Request - Copy Date of service to duplicate attribute'
set @formula = '("[0d989b51-3bf4-482a-974e-d95d06c62db6]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Date of service_validate'
set @criteriaFieldID = '3715c351-2d0f-4b94-b65f-f9ec74cbc529'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Serviced by to duplicate attribute'
set @tempdescription = 'New Request - Copy Serviced by to duplicate attribute'
set @formula = '("[cede4f51-9369-48ab-95c3-944ed81d4e17]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Serviced by_validate'
set @criteriaFieldID = 'ae1b02bc-32cb-44cb-a5c4-0a81a8ca76a6'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a service document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Document to duplicate attribute'
set @tempdescription = 'New Request - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a tax document review'
set @tempdescription = 'If its a tax document review'
set @relatedStepDescription = 'If its an insurance document review'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a tax document review'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempTaxValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Checked by vehicle owner to duplicate attribute'
set @tempdescription = 'New Request - Copy Checked by vehicle owner to duplicate attribute'
set @formula = '("[ee6447bd-b4ad-4b9a-a34f-b365aab9e369]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Checked by vehicle owner_validate'
set @criteriaFieldID = '44bae7eb-4f77-4fd0-83d7-38440526a7f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy SORN to duplicate attribute'
set @tempdescription = 'New Request - Copy SORN to duplicate attribute'
set @formula = '("[ec938d48-89d2-40f1-a06c-8c5602ca5dad]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'SORN_validate'
set @criteriaFieldID = 'f29ccb04-6214-48f3-b1c7-696c90319f51'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a tax document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Document to duplicate attribute'
set @tempdescription = 'New Request - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a breakdown cover document review'
set @tempdescription = 'If its a breakdown cover document review'
set @relatedStepDescription = 'If its an insurance document review'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a breakdown cover document review'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'type'
set @tempcondition = 1
set @tempvalue1 = @tempBreakDownValue
set @tempvalue2 = ''
set @criteriaFieldID = '0107c206-185d-4cb6-8bb3-9b561ac80cfa'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Provider to duplicate attribute'
set @tempdescription = 'New Request - Copy Provider to duplicate attribute'
set @formula = '("[27109f54-4305-4b64-896c-94c6a15b52c0]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Provider_validate'
set @criteriaFieldID = 'a5e78abe-a398-473e-8a63-f734d9084e7e'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Checked by vehicle owner to duplicate attribute'
set @tempdescription = 'New Request - Copy Checked by vehicle owner to duplicate attribute'
set @formula = '("[ee6447bd-b4ad-4b9a-a34f-b365aab9e369]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Checked by vehicle owner_validate'
set @criteriaFieldID = '44bae7eb-4f77-4fd0-83d7-38440526a7f0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a breakdown cover document review'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step New Request - Copy Document to duplicate attribute'
set @tempdescription = 'New Request - Copy Document to duplicate attribute'
set @formula = '("[3e9ed916-08c9-4192-ad29-b2103b458d26]")'
set @tablename = 'Vehicle Document'
set @fieldname = 'Document_validate'
set @criteriaFieldID = 'caabb9ab-6e40-4eb3-9f4a-014a24f4c5e0'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Check if the user is reviewer whos editing the form'
set @tempdescription = 'Check if the user is reviewer whos editing the form'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Check if the user is reviewer whos editing the form'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Reviewed by'
set @tempcondition = 2
set @tempvalue1 = 'DEB645A2-1A9D-48E2-8801-46E7A69802AC'
set @tempvalue2 = ''
set @criteriaFieldID = '80591948-5246-4d25-b606-c99a93d5f911'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whos editing the form'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Send Email to Notify Reviewer that new Service document has been submitted by a claimant that requires review'
set @tempdescription = 'Send Email to Notify Reviewer that new Service document has been submitted by a claimant that requires review'
set @tempemailtemplatename = 'Send to an approver when claimant adds a new vehicle document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Block Approver From Getting Mails'
set @tempdescription = 'Block Approver From Getting Mails'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Block Approver From Getting Mails'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'ApproverCheck'
set @tempcondition = 1
set @tempvalue1 = '0'
set @tempvalue2 = ''
set @criteriaFieldID = 'f5ace912-2c70-4e56-bf56-62a05e0f62ff'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Block Approver From Getting Mails'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Send Email to Notify Reviewer that new Service document has been submitted by a claimant that requires review'
set @tempdescription = 'Send Email to Notify Reviewer that new Service document has been submitted by a claimant that requires review'
set @tempemailtemplatename = 'Send to an approver when claimant adds a new vehicle document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - when the status is not invalidated'
set @tempdescription = 'If claimant modifies the DOC details - when the status is not invalidated'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - when the status is not invalidated'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @tempcondition = 1
set @tempvalue1 = @tempInvalidatedValue
set @tempvalue2 = ''
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - when the status is not invalidated'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow if no values are changed'
set @tempdescription = 'Finish Workflow if no values are changed'
set @relatedStepDescription = 'If claimant modifies the DOC details - when the status is not invalidated'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 10, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Finish Workflow if no values are changed'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @parentdescription = 'Finish Workflow if no values are changed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Awaiting Review'
set @tempdescription = 'Set Review to Awaiting Review'
set @formula = @tempAwaitingReviewValue
set @tablename = 'Vehicle Document'
set @fieldname = 'Status'
set @criteriaFieldID = '9075f93b-3cc8-47e2-98da-ac44d1eda431'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Finish Workflow if no values are changed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow if its an editing request from approver'
set @tempdescription = 'Finish Workflow if its an editing request from approver'
set @relatedStepDescription = 'If claimant modifies the DOC details - when the status is not invalidated'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 10, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Finish Workflow if its an editing request from approver'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @parentdescription = 'Finish Workflow if its an editing request from approver'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null

go



IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'The database update succeeded';
COMMIT TRANSACTION
END
ELSE
PRINT 'The database update failed for ' + DB_NAME()
GO
DROP TABLE #tmpErrors
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#stepIds')) DROP TABLE #stepIds;
IF OBJECT_ID('tempdb..#GetAttributeListValue') IS NOT NULL DROP PROCEDURE #GetAttributeListValue
GO
UPDATE workflows
SET baseTableID = '28D592D7-4596-49C4-96B8-45655D8DF797'
WHERE workflowName = 'Notify Claimant and Approver On Duty Of Care Documents Review Status and requests'
GO
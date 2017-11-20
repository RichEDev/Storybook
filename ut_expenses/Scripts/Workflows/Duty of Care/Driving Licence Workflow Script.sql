DELETE
FROM workflows
WHERE workflowName = 'Notify Claimant and Approver On Duty Of Care Driving License Review Status'
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
PRINT '------- DoC Driving Licence Documents Workflow Migration Script Customer -------';
GO

IF EXISTS (
		SELECT *
		FROM workflows
		WHERE workflowName = 'Notify Claimant and Approver On Duty Of Care Driving License Review Status'
		)
	DELETE
	FROM workflows
	WHERE workflowName = 'Notify Claimant and Approver On Duty Of Care Driving License Review Status'

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
 declare @tempPhotoCardValue nvarchar(200)
 declare @tempPaperValue nvarchar(200)
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
set @tempworkflowname = 'Notify Claimant and Approver On Duty Of Care Driving License Review Status'
set @tempbasetableid = 'F066EF8F-705F-4CD7-8DD7-FDEFBF8F3821'
exec @tempworkflowID = saveWorkflow @workflowid = 0, @workflowname = @tempworkflowname, @description = @tempworkflowdescription, @runoncreation = 1, @runonchange = 1, @runondelete = 0, @basetableID = @tempbasetableid, @canbechildworkflow = 0, @workflowtype = 5, @activeuser = @employeeid, @CUemployeeID = @employeeid, @CUdelegateID = null
exec @tempPhotoCardValue =  #GetAttributeListValue 'CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE','Photocard' 
 exec @tempPaperValue =  #GetAttributeListValue 'CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE','Pre-1998 Paper Driving Licence' 
 exec @tempAwaitingReviewValue =  #GetAttributeListValue 'EB913CB2-C56B-4461-924E-833C43DD6F65','Awaiting Review' 
 exec @tempInvalidatedValue =  #GetAttributeListValue 'EB913CB2-C56B-4461-924E-833C43DD6F65','Invalidated' 
 exec @tempFailedValue =  #GetAttributeListValue 'EB913CB2-C56B-4461-924E-833C43DD6F65','Reviewed - Failed' 
 exec @tempOkValue =  #GetAttributeListValue 'EB913CB2-C56B-4461-924E-833C43DD6F65','Reviewed - OK' 
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
print 'Creating workflow step If the review status = Failed '
set @tempdescription = 'If the review status = Failed '
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If the review status = Failed '' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @tempcondition = 1
set @tempvalue1 = @tempFailedValue
set @tempvalue2 = ''
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the review status = Failed '
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Check if the user is reviewer if status is failed'
set @tempdescription = 'Check if the user is reviewer if status is failed'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Check if the user is reviewer if status is failed'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Reviewed by'
set @tempcondition = 1
set @tempvalue1 = 'ddc48a05-0d47-4a05-a3d9-488fe15e5ae5'
set @tempvalue2 = ''
set @criteriaFieldID = 'e3d6bb3b-dbcd-4bf9-b5bb-5dc3c2bfa5ae'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer if status is failed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Send Email to Notify claimant if the approver is unhappy with the driving license document'
set @tempdescription = 'Send Email to Notify claimant if the approver is unhappy with the driving license document'
set @tempemailtemplatename = 'Send to the claimant if the approver is unhappy with the driving licence document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer if status is failed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set the Review Date'
set @tempdescription = 'Set the Review Date'
set @formula = 'TEXT(NOW(),"yyyy-MM-dd HH:mm:ss.fff")'
set @tablename = 'Driving Licence'
set @fieldname = 'Review date'
set @criteriaFieldID = '4568ea04-3bd0-40db-9197-543ded6a6527'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer if status is failed'
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
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @tempcondition = 1
set @tempvalue1 = @tempOkValue
set @tempvalue2 = ''
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the review is good'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Assign Approver Check'
set @tempdescription = 'Assign Approver Check'
set @formula = 'IF("[ddc48a05-0d47-4a05-a3d9-488fe15e5ae5]">" ",1,0)'
set @tablename = 'Driving Licence'
set @fieldname = 'ApproverCheck'
set @criteriaFieldID = 'a7043be5-1abe-44a9-889e-a7bc10587403'
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
set @tablename = 'Driving Licence'
set @fieldname = 'ApproverCheck'
set @tempcondition = 1
set @tempvalue1 = '1'
set @tempvalue2 = ''
set @criteriaFieldID = 'a7043be5-1abe-44a9-889e-a7bc10587403'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if its a new docuemnt uploaded by reviewer setting review to OK'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Assign Approver Check - second condition'
set @tempdescription = 'Assign Approver Check - second condition'
set @formula = 'IF("[e3d6bb3b-dbcd-4bf9-b5bb-5dc3c2bfa5ae]">" ",1,2)'
set @tablename = 'Driving Licence'
set @fieldname = 'ApproverCheck'
set @criteriaFieldID = 'a7043be5-1abe-44a9-889e-a7bc10587403'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the review is good'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Check if the reviewer is null - data migration scenario'
set @tempdescription = 'Check if the reviewer is null - data migration scenario'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Check if the reviewer is null - data migration scenario'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'ApproverCheck'
set @tempcondition = 1
set @tempvalue1 = '2'
set @tempvalue2 = ''
set @criteriaFieldID = 'a7043be5-1abe-44a9-889e-a7bc10587403'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the reviewer is null - data migration scenario'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC data migrated details - Check code?'
set @tempdescription = 'If claimant modifies the DOC data migrated details - Check code?'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC data migrated details - Check code?'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Check code_validate'
set @tempcondition = 2
set @tempvalue1 = 'c6567181-9f8e-49c7-8063-4e73ce347564'
set @tempvalue2 = ''
set @criteriaFieldID = '4d302354-9ba4-48a6-8512-de9054ccc9b2'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Check code?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Check code to duplicate attribute'
set @tempdescription = 'Copy Check code to duplicate attribute'
set @formula = '("[c6567181-9f8e-49c7-8063-4e73ce347564]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Check code_validate'
set @criteriaFieldID = '4d302354-9ba4-48a6-8512-de9054ccc9b2'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Check code?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the reviewer is null - data migration scenario'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC data migrated details - Check Code Expiry Date?'
set @tempdescription = 'If claimant modifies the DOC data migrated details - Check Code Expiry Date?'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC data migrated details - Check Code Expiry Date?'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Check Code Expiry Date_validate'
set @tempcondition = 2
set @tempvalue1 = '3e9ac071-04bf-44e2-96c4-c3c417e2a88b'
set @tempvalue2 = ''
set @criteriaFieldID = 'ef14b850-0f39-4244-a23b-887df2cd3cdc'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Check Code Expiry Date?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Check Code Expiry Date to duplicate attribute'
set @tempdescription = 'Copy Check Code Expiry Date to duplicate attribute'
set @formula = '("[3e9ac071-04bf-44e2-96c4-c3c417e2a88b]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Check Code Expiry Date_validate'
set @criteriaFieldID = 'ef14b850-0f39-4244-a23b-887df2cd3cdc'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Check Code Expiry Date?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the reviewer is null - data migration scenario'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC data migrated details - Date of issue?'
set @tempdescription = 'If claimant modifies the DOC data migrated details - Date of issue?'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC data migrated details - Date of issue?'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Date of issue_validate'
set @tempcondition = 2
set @tempvalue1 = '23bae96e-bdcd-455e-bee2-e0d4ba4fd53d'
set @tempvalue2 = ''
set @criteriaFieldID = '73955a17-ea39-40e2-ab65-4b835a1f5016'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Date of issue?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Date of issue to duplicate attribute'
set @tempdescription = 'Copy Date of issue to duplicate attribute'
set @formula = '("[23bae96e-bdcd-455e-bee2-e0d4ba4fd53d]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Date of issue_validate'
set @criteriaFieldID = '73955a17-ea39-40e2-ab65-4b835a1f5016'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Date of issue?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the reviewer is null - data migration scenario'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC data migrated details - Issue Number'
set @tempdescription = 'If claimant modifies the DOC data migrated details - Issue Number'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC data migrated details - Issue Number'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Issue Number_validate'
set @tempcondition = 2
set @tempvalue1 = 'f55790e3-812a-47f5-8f6d-8103c82ec286'
set @tempvalue2 = ''
set @criteriaFieldID = '898aec93-3671-4c46-aca8-3cb7f0541fec'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Issue Number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Issue Number to duplicate attribute'
set @tempdescription = 'Copy Issue Number to duplicate attribute'
set @formula = '("[f55790e3-812a-47f5-8f6d-8103c82ec286]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Issue Number_validate'
set @criteriaFieldID = '898aec93-3671-4c46-aca8-3cb7f0541fec'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Issue Number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the reviewer is null - data migration scenario'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC data migrated details - Issuing authority'
set @tempdescription = 'If claimant modifies the DOC data migrated details - Issuing authority'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC data migrated details - Issuing authority'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Issuing authority_validate'
set @tempcondition = 2
set @tempvalue1 = '1e5a0ad0-d5a3-4304-8381-b937bed9bb8b'
set @tempvalue2 = ''
set @criteriaFieldID = '0eb394b1-52fe-4639-b1fe-14920fa8e50f'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Issuing authority'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Issuing authority to duplicate attribute'
set @tempdescription = 'Copy Issuing authority to duplicate attribute'
set @formula = '("[1e5a0ad0-d5a3-4304-8381-b937bed9bb8b]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Issuing authority_validate'
set @criteriaFieldID = '0eb394b1-52fe-4639-b1fe-14920fa8e50f'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Issuing authority'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the reviewer is null - data migration scenario'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC data migrated details - Licence number'
set @tempdescription = 'If claimant modifies the DOC data migrated details - Licence number'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC data migrated details - Licence number'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Licence number_validate'
set @tempcondition = 2
set @tempvalue1 = '10dfd838-201e-4258-a6b9-259bfc137062'
set @tempvalue2 = ''
set @criteriaFieldID = '87c3c54e-6f55-4786-b180-10705edffa20'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Licence number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Licence numbery to duplicate attribute'
set @tempdescription = 'Copy Licence numbery to duplicate attribute'
set @formula = '("[10dfd838-201e-4258-a6b9-259bfc137062]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Licence number_validate'
set @criteriaFieldID = '87c3c54e-6f55-4786-b180-10705edffa20'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Licence number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the reviewer is null - data migration scenario'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a Photo Licence data migrated edit'
set @tempdescription = 'If its a Photo Licence data migrated edit'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a Photo Licence data migrated edit'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Licence type'
set @tempcondition = 1
set @tempvalue1 = @tempPhotoCardValue
set @tempvalue2 = ''
set @criteriaFieldID = '63a7f8ad-4391-4b86-839d-798389ceba1b'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Photo Licence data migrated edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC data migrated details - Photocard Front'
set @tempdescription = 'If claimant modifies the DOC data migrated details - Photocard Front'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC data migrated details - Photocard Front'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Photocard Front_validate'
set @tempcondition = 2
set @tempvalue1 = '3210038d-7db4-45e4-b502-5587e597eeb5'
set @tempvalue2 = ''
set @criteriaFieldID = '67c50884-4355-49c2-904d-544b20a4a590'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Photocard Front'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Photocard Front to duplicate attribute'
set @tempdescription = 'Copy Photocard Front to duplicate attribute'
set @formula = '("[3210038d-7db4-45e4-b502-5587e597eeb5]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Photocard Front_validate'
set @criteriaFieldID = '67c50884-4355-49c2-904d-544b20a4a590'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Photocard Front'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Photo Licence data migrated edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC data migrated details - Photocard Back'
set @tempdescription = 'If claimant modifies the DOC data migrated details - Photocard Back'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC data migrated details - Photocard Back'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Photocard Back_validate'
set @tempcondition = 2
set @tempvalue1 = '18a3c39e-8c83-4bda-b224-58648a4262ea'
set @tempvalue2 = ''
set @criteriaFieldID = 'ccd9f9d7-276b-466b-9ceb-0f313a96ab90'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Photocard Back'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Photocard Back to duplicate attribute'
set @tempdescription = 'Copy Photocard Back to duplicate attribute'
set @formula = '("[18a3c39e-8c83-4bda-b224-58648a4262ea]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Photocard Back_validate'
set @criteriaFieldID = 'ccd9f9d7-276b-466b-9ceb-0f313a96ab90'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Photocard Back'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Photo Licence data migrated edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC data migrated details - Expiry date'
set @tempdescription = 'If claimant modifies the DOC data migrated details - Expiry date'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC data migrated details - Expiry date'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Expiry date_validate'
set @tempcondition = 2
set @tempvalue1 = '07c09062-68c2-4e1a-863a-c8fa94911288'
set @tempvalue2 = ''
set @criteriaFieldID = 'efc2ac8f-5746-410d-959b-4694b03cccff'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Expiry date'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Expiry date to duplicate attribute'
set @tempdescription = 'Copy Expiry date to duplicate attribute'
set @formula = '("[07c09062-68c2-4e1a-863a-c8fa94911288]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Expiry date_validate'
set @criteriaFieldID = 'efc2ac8f-5746-410d-959b-4694b03cccff'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Expiry date'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the reviewer is null - data migration scenario'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a Paper Licence data migrated edit'
set @tempdescription = 'If its a Paper Licence data migrated edit'
set @relatedStepDescription = 'If its a Photo Licence data migrated edit'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a Paper Licence data migrated edit'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Licence type'
set @tempcondition = 1
set @tempvalue1 = @tempPaperValue
set @tempvalue2 = ''
set @criteriaFieldID = '63a7f8ad-4391-4b86-839d-798389ceba1b'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Paper Licence data migrated edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC data migrated details - Document of photo card'
set @tempdescription = 'If claimant modifies the DOC data migrated details - Document of photo card'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC data migrated details - Document of photo card'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Document_validate'
set @tempcondition = 2
set @tempvalue1 = '6dd910d7-01df-4a42-b0a8-ddc8ca0bbb0e'
set @tempvalue2 = ''
set @criteriaFieldID = '7fef1ddf-73d7-47d0-96ca-5b8dd3f62e6c'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Document of photo card'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Photocard Back to duplicate attribute'
set @tempdescription = 'Copy Photocard Back to duplicate attribute'
set @formula = '("[6dd910d7-01df-4a42-b0a8-ddc8ca0bbb0e]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Document_validate'
set @criteriaFieldID = '7fef1ddf-73d7-47d0-96ca-5b8dd3f62e6c'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC data migrated details - Document of photo card'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the reviewer is null - data migration scenario'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the data migrated Driving Licence details'
set @tempdescription = 'If claimant modifies the data migrated Driving Licence details'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the data migrated Driving Licence details'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @tempcondition = 1
set @tempvalue1 = @tempInvalidatedValue
set @tempvalue2 = ''
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the data migrated Driving Licence details'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Invalidated review'
set @tempdescription = 'Invalidated review'
set @tempemailtemplatename = 'Send to an approver when claimant modifies a driving licence document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the data migrated Driving Licence details'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the reviewer is null - data migration scenario'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow After Values are assigned after data migrated'
set @tempdescription = 'Finish Workflow After Values are assigned after data migrated'
set @relatedStepDescription = 'If claimant modifies the data migrated Driving Licence details'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 10, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Finish Workflow After Values are assigned after data migrated'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @parentdescription = 'Finish Workflow After Values are assigned after data migrated'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the review is good'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Check if the user is reviewer whilst uploading new doc'
set @tempdescription = 'Check if the user is reviewer whilst uploading new doc'
set @relatedStepDescription = 'Check if the reviewer is null - data migration scenario'
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
set @tablename = 'Driving Licence'
set @fieldname = 'ApproverCheck'
set @tempcondition = 1
set @tempvalue1 = '0'
set @tempvalue2 = ''
set @criteriaFieldID = 'a7043be5-1abe-44a9-889e-a7bc10587403'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set the Review Date'
set @tempdescription = 'Set the Review Date'
set @formula = 'TEXT(NOW(),"yyyy-MM-dd HH:mm:ss.fff")'
set @tablename = 'Driving Licence'
set @fieldname = 'Review date'
set @criteriaFieldID = '4568ea04-3bd0-40db-9197-543ded6a6527'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Check Code to duplicate attribute'
set @tempdescription = 'Copy Check Code to duplicate attribute'
set @formula = '("[c6567181-9f8e-49c7-8063-4e73ce347564]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Check Code_validate'
set @criteriaFieldID = '4d302354-9ba4-48a6-8512-de9054ccc9b2'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Check Code to duplicate attribute'
set @tempdescription = 'Copy Check Code to duplicate attribute'
set @formula = '("[3e9ac071-04bf-44e2-96c4-c3c417e2a88b]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Check Code Expiry Date_validate'
set @criteriaFieldID = 'ef14b850-0f39-4244-a23b-887df2cd3cdc'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Date of issue to duplicate attribute'
set @tempdescription = 'Copy Date of issue to duplicate attribute'
set @formula = '("[23bae96e-bdcd-455e-bee2-e0d4ba4fd53d]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Date of issue_validate'
set @criteriaFieldID = '73955a17-ea39-40e2-ab65-4b835a1f5016'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Issue Number to duplicate attribute'
set @tempdescription = 'Copy Issue Number to duplicate attribute'
set @formula = '("[f55790e3-812a-47f5-8f6d-8103c82ec286]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Issue Number_validate'
set @criteriaFieldID = '898aec93-3671-4c46-aca8-3cb7f0541fec'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Issuing authority to duplicate attribute'
set @tempdescription = 'Copy Issuing authority to duplicate attribute'
set @formula = '("[1e5a0ad0-d5a3-4304-8381-b937bed9bb8b]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Issuing authority_validate'
set @criteriaFieldID = '0eb394b1-52fe-4639-b1fe-14920fa8e50f'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Licence numbery to duplicate attribute'
set @tempdescription = 'Copy Licence numbery to duplicate attribute'
set @formula = '("[10dfd838-201e-4258-a6b9-259bfc137062]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Licence number_validate'
set @criteriaFieldID = '87c3c54e-6f55-4786-b180-10705edffa20'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a Photo Licence edit by reviewer - new request'
set @tempdescription = 'If its a Photo Licence edit by reviewer - new request'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a Photo Licence edit by reviewer - new request'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Licence type'
set @tempcondition = 1
set @tempvalue1 = @tempPhotoCardValue
set @tempvalue2 = ''
set @criteriaFieldID = '63a7f8ad-4391-4b86-839d-798389ceba1b'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Photo Licence edit by reviewer - new request'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Photocard Front to duplicate attribute'
set @tempdescription = 'Copy Photocard Front to duplicate attribute'
set @formula = '("[3210038d-7db4-45e4-b502-5587e597eeb5]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Photocard Front_validate'
set @criteriaFieldID = '67c50884-4355-49c2-904d-544b20a4a590'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Photo Licence edit by reviewer - new request'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Photocard Back to duplicate attribute'
set @tempdescription = 'Copy Photocard Back to duplicate attribute'
set @formula = '("[18a3c39e-8c83-4bda-b224-58648a4262ea]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Photocard Back_validate'
set @criteriaFieldID = 'ccd9f9d7-276b-466b-9ceb-0f313a96ab90'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Photo Licence edit by reviewer - new request'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Expiry date to duplicate attribute'
set @tempdescription = 'Copy Expiry date to duplicate attribute'
set @formula = '("[07c09062-68c2-4e1a-863a-c8fa94911288]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Expiry date_validate'
set @criteriaFieldID = 'efc2ac8f-5746-410d-959b-4694b03cccff'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a Paper Licence edit by reviewer - new request'
set @tempdescription = 'If its a Paper Licence edit by reviewer - new request'
set @relatedStepDescription = 'If its a Photo Licence edit by reviewer - new request'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a Paper Licence edit by reviewer - new request'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Licence type'
set @tempcondition = 1
set @tempvalue1 = @tempPaperValue
set @tempvalue2 = ''
set @criteriaFieldID = '63a7f8ad-4391-4b86-839d-798389ceba1b'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Paper Licence edit by reviewer - new request'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Document to duplicate attribute'
set @tempdescription = 'Copy Document to duplicate attribute'
set @formula = '("[6dd910d7-01df-4a42-b0a8-ddc8ca0bbb0e]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Document_validate'
set @criteriaFieldID = '7fef1ddf-73d7-47d0-96ca-5b8dd3f62e6c'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst uploading new doc'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow After Values are updated in the new request'
set @tempdescription = 'Finish Workflow After Values are updated in the new request'
set @relatedStepDescription = 'If its a Photo Licence edit by reviewer - new request'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 10, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Finish Workflow After Values are updated in the new request'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @parentdescription = 'Finish Workflow After Values are updated in the new request'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the review is good'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Check if the user is reviewer whilst the review is passed'
set @tempdescription = 'Check if the user is reviewer whilst the review is passed'
set @relatedStepDescription = 'Check if the reviewer is null - data migration scenario'
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
set @tablename = 'Driving Licence'
set @fieldname = 'Reviewed by'
set @tempcondition = 1
set @tempvalue1 = 'ddc48a05-0d47-4a05-a3d9-488fe15e5ae5'
set @tempvalue2 = ''
set @criteriaFieldID = 'e3d6bb3b-dbcd-4bf9-b5bb-5dc3c2bfa5ae'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set the Review Date'
set @tempdescription = 'Set the Review Date'
set @formula = 'TEXT(NOW(),"yyyy-MM-dd HH:mm:ss.fff")'
set @tablename = 'Driving Licence'
set @fieldname = 'Review date'
set @criteriaFieldID = '4568ea04-3bd0-40db-9197-543ded6a6527'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Check Code to duplicate attribute'
set @tempdescription = 'Copy Check Code to duplicate attribute'
set @formula = '("[c6567181-9f8e-49c7-8063-4e73ce347564]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Check Code_validate'
set @criteriaFieldID = '4d302354-9ba4-48a6-8512-de9054ccc9b2'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Check Code to duplicate attribute'
set @tempdescription = 'Copy Check Code to duplicate attribute'
set @formula = '("[3e9ac071-04bf-44e2-96c4-c3c417e2a88b]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Check Code Expiry Date_validate'
set @criteriaFieldID = 'ef14b850-0f39-4244-a23b-887df2cd3cdc'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Date of issue to duplicate attribute'
set @tempdescription = 'Copy Date of issue to duplicate attribute'
set @formula = '("[23bae96e-bdcd-455e-bee2-e0d4ba4fd53d]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Date of issue_validate'
set @criteriaFieldID = '73955a17-ea39-40e2-ab65-4b835a1f5016'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Issue Number to duplicate attribute'
set @tempdescription = 'Copy Issue Number to duplicate attribute'
set @formula = '("[f55790e3-812a-47f5-8f6d-8103c82ec286]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Issue Number_validate'
set @criteriaFieldID = '898aec93-3671-4c46-aca8-3cb7f0541fec'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Issuing authority to duplicate attribute'
set @tempdescription = 'Copy Issuing authority to duplicate attribute'
set @formula = '("[1e5a0ad0-d5a3-4304-8381-b937bed9bb8b]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Issuing authority_validate'
set @criteriaFieldID = '0eb394b1-52fe-4639-b1fe-14920fa8e50f'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Licence numbery to duplicate attribute'
set @tempdescription = 'Copy Licence numbery to duplicate attribute'
set @formula = '("[10dfd838-201e-4258-a6b9-259bfc137062]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Licence number_validate'
set @criteriaFieldID = '87c3c54e-6f55-4786-b180-10705edffa20'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)

print 'Creating workflow step Send Email to Notify claimant if the approver has approved the driving licence'
set @tempdescription = 'Send Email to Notify claimant if the approver has approved the driving licence'
set @tempemailtemplatename = 'Send to the claimant if the approver is has approved driving licence document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)

print 'Creating workflow step If its a Photo Licence edit by reviewer'
set @tempdescription = 'If its a Photo Licence edit by reviewer'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a Photo Licence edit by reviewer'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Licence type'
set @tempcondition = 1
set @tempvalue1 = @tempPhotoCardValue
set @tempvalue2 = ''
set @criteriaFieldID = '63a7f8ad-4391-4b86-839d-798389ceba1b'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Photo Licence edit by reviewer'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Photocard Front to duplicate attribute'
set @tempdescription = 'Copy Photocard Front to duplicate attribute'
set @formula = '("[3210038d-7db4-45e4-b502-5587e597eeb5]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Photocard Front_validate'
set @criteriaFieldID = '67c50884-4355-49c2-904d-544b20a4a590'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Photo Licence edit by reviewer'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Photocard Back to duplicate attribute'
set @tempdescription = 'Copy Photocard Back to duplicate attribute'
set @formula = '("[18a3c39e-8c83-4bda-b224-58648a4262ea]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Photocard Back_validate'
set @criteriaFieldID = 'ccd9f9d7-276b-466b-9ceb-0f313a96ab90'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Photo Licence edit by reviewer'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Expiry date to duplicate attribute'
set @tempdescription = 'Copy Expiry date to duplicate attribute'
set @formula = '("[07c09062-68c2-4e1a-863a-c8fa94911288]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Expiry date_validate'
set @criteriaFieldID = 'efc2ac8f-5746-410d-959b-4694b03cccff'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a Paper Licence edit by reviewer'
set @tempdescription = 'If its a Paper Licence edit by reviewer'
set @relatedStepDescription = 'If its a Photo Licence edit by reviewer'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a Paper Licence edit by reviewer'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Licence type'
set @tempcondition = 1
set @tempvalue1 = @tempPaperValue
set @tempvalue2 = ''
set @criteriaFieldID = '63a7f8ad-4391-4b86-839d-798389ceba1b'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Paper Licence edit by reviewer'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Document to duplicate attribute'
set @tempdescription = 'Copy Document to duplicate attribute'
set @formula = '("[6dd910d7-01df-4a42-b0a8-ddc8ca0bbb0e]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Document_validate'
set @criteriaFieldID = '7fef1ddf-73d7-47d0-96ca-5b8dd3f62e6c'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whilst the review is passed'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow After Values are updated'
set @tempdescription = 'Finish Workflow After Values are updated'
set @relatedStepDescription = 'If its a Photo Licence edit by reviewer'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 10, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Finish Workflow After Values are updated'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @parentdescription = 'Finish Workflow After Values are updated'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the review is good'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If the user is claimant'
set @tempdescription = 'If the user is claimant'
set @relatedStepDescription = 'Check if the reviewer is null - data migration scenario'
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
set @tablename = 'Driving Licence'
set @fieldname = 'Reviewed by'
set @tempcondition = 2
set @tempvalue1 = 'ddc48a05-0d47-4a05-a3d9-488fe15e5ae5'
set @tempvalue2 = ''
set @criteriaFieldID = 'e3d6bb3b-dbcd-4bf9-b5bb-5dc3c2bfa5ae'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Check code?'
set @tempdescription = 'If claimant modifies the DOC details - Check code?'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Check code?'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Check code_validate'
set @tempcondition = 2
set @tempvalue1 = 'c6567181-9f8e-49c7-8063-4e73ce347564'
set @tempvalue2 = ''
set @criteriaFieldID = '4d302354-9ba4-48a6-8512-de9054ccc9b2'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Check code?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Check code to duplicate attribute'
set @tempdescription = 'Copy Check code to duplicate attribute'
set @formula = '("[c6567181-9f8e-49c7-8063-4e73ce347564]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Check code_validate'
set @criteriaFieldID = '4d302354-9ba4-48a6-8512-de9054ccc9b2'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Check code?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Check Code Expiry Date?'
set @tempdescription = 'If claimant modifies the DOC details - Check Code Expiry Date?'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Check Code Expiry Date?'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Check Code Expiry Date_validate'
set @tempcondition = 2
set @tempvalue1 = '3e9ac071-04bf-44e2-96c4-c3c417e2a88b'
set @tempvalue2 = ''
set @criteriaFieldID = 'ef14b850-0f39-4244-a23b-887df2cd3cdc'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Check Code Expiry Date?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Check Code Expiry Date to duplicate attribute'
set @tempdescription = 'Copy Check Code Expiry Date to duplicate attribute'
set @formula = '("[3e9ac071-04bf-44e2-96c4-c3c417e2a88b]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Check Code Expiry Date_validate'
set @criteriaFieldID = 'ef14b850-0f39-4244-a23b-887df2cd3cdc'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Check Code Expiry Date?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Date of issue?'
set @tempdescription = 'If claimant modifies the DOC details - Date of issue?'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Date of issue?'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Date of issue_validate'
set @tempcondition = 2
set @tempvalue1 = '23bae96e-bdcd-455e-bee2-e0d4ba4fd53d'
set @tempvalue2 = ''
set @criteriaFieldID = '73955a17-ea39-40e2-ab65-4b835a1f5016'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Date of issue?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Date of issue to duplicate attribute'
set @tempdescription = 'Copy Date of issue to duplicate attribute'
set @formula = '("[23bae96e-bdcd-455e-bee2-e0d4ba4fd53d]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Date of issue_validate'
set @criteriaFieldID = '73955a17-ea39-40e2-ab65-4b835a1f5016'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Date of issue?'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Issue Number'
set @tempdescription = 'If claimant modifies the DOC details - Issue Number'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Issue Number'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Issue Number_validate'
set @tempcondition = 2
set @tempvalue1 = 'f55790e3-812a-47f5-8f6d-8103c82ec286'
set @tempvalue2 = ''
set @criteriaFieldID = '898aec93-3671-4c46-aca8-3cb7f0541fec'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Issue Number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Issue Number to duplicate attribute'
set @tempdescription = 'Copy Issue Number to duplicate attribute'
set @formula = '("[f55790e3-812a-47f5-8f6d-8103c82ec286]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Issue Number_validate'
set @criteriaFieldID = '898aec93-3671-4c46-aca8-3cb7f0541fec'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Issue Number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Issuing authority'
set @tempdescription = 'If claimant modifies the DOC details - Issuing authority'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Issuing authority'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Issuing authority_validate'
set @tempcondition = 2
set @tempvalue1 = '1e5a0ad0-d5a3-4304-8381-b937bed9bb8b'
set @tempvalue2 = ''
set @criteriaFieldID = '0eb394b1-52fe-4639-b1fe-14920fa8e50f'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Issuing authority'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Issuing authority to duplicate attribute'
set @tempdescription = 'Copy Issuing authority to duplicate attribute'
set @formula = '("[1e5a0ad0-d5a3-4304-8381-b937bed9bb8b]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Issuing authority_validate'
set @criteriaFieldID = '0eb394b1-52fe-4639-b1fe-14920fa8e50f'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Issuing authority'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Licence number'
set @tempdescription = 'If claimant modifies the DOC details - Licence number'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Licence number'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Licence number_validate'
set @tempcondition = 2
set @tempvalue1 = '10dfd838-201e-4258-a6b9-259bfc137062'
set @tempvalue2 = ''
set @criteriaFieldID = '87c3c54e-6f55-4786-b180-10705edffa20'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Licence number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Licence numbery to duplicate attribute'
set @tempdescription = 'Copy Licence numbery to duplicate attribute'
set @formula = '("[10dfd838-201e-4258-a6b9-259bfc137062]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Licence number_validate'
set @criteriaFieldID = '87c3c54e-6f55-4786-b180-10705edffa20'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Licence number'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a Photo Licence edit'
set @tempdescription = 'If its a Photo Licence edit'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a Photo Licence edit'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Licence type'
set @tempcondition = 1
set @tempvalue1 = @tempPhotoCardValue
set @tempvalue2 = ''
set @criteriaFieldID = '63a7f8ad-4391-4b86-839d-798389ceba1b'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Photo Licence edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Photocard Front'
set @tempdescription = 'If claimant modifies the DOC details - Photocard Front'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Photocard Front'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Photocard Front_validate'
set @tempcondition = 2
set @tempvalue1 = '3210038d-7db4-45e4-b502-5587e597eeb5'
set @tempvalue2 = ''
set @criteriaFieldID = '67c50884-4355-49c2-904d-544b20a4a590'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Photocard Front'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Photocard Front to duplicate attribute'
set @tempdescription = 'Copy Photocard Front to duplicate attribute'
set @formula = '("[3210038d-7db4-45e4-b502-5587e597eeb5]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Photocard Front_validate'
set @criteriaFieldID = '67c50884-4355-49c2-904d-544b20a4a590'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Photocard Front'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Photo Licence edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Photocard Back'
set @tempdescription = 'If claimant modifies the DOC details - Photocard Back'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Photocard Back'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Photocard Back_validate'
set @tempcondition = 2
set @tempvalue1 = '18a3c39e-8c83-4bda-b224-58648a4262ea'
set @tempvalue2 = ''
set @criteriaFieldID = 'ccd9f9d7-276b-466b-9ceb-0f313a96ab90'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Photocard Back'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Photocard Back to duplicate attribute'
set @tempdescription = 'Copy Photocard Back to duplicate attribute'
set @formula = '("[18a3c39e-8c83-4bda-b224-58648a4262ea]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Photocard Back_validate'
set @criteriaFieldID = 'ccd9f9d7-276b-466b-9ceb-0f313a96ab90'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Photocard Back'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Photo Licence edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Expiry date'
set @tempdescription = 'If claimant modifies the DOC details - Expiry date'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Expiry date'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Expiry date_validate'
set @tempcondition = 2
set @tempvalue1 = '07c09062-68c2-4e1a-863a-c8fa94911288'
set @tempvalue2 = ''
set @criteriaFieldID = 'efc2ac8f-5746-410d-959b-4694b03cccff'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Expiry date'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Expiry date to duplicate attribute'
set @tempdescription = 'Copy Expiry date to duplicate attribute'
set @formula = '("[07c09062-68c2-4e1a-863a-c8fa94911288]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Expiry date_validate'
set @criteriaFieldID = 'efc2ac8f-5746-410d-959b-4694b03cccff'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Expiry date'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a Paper Licence edit'
set @tempdescription = 'If its a Paper Licence edit'
set @relatedStepDescription = 'If its a Photo Licence edit'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a Paper Licence edit'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Licence type'
set @tempcondition = 1
set @tempvalue1 = @tempPaperValue
set @tempvalue2 = ''
set @criteriaFieldID = '63a7f8ad-4391-4b86-839d-798389ceba1b'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a Paper Licence edit'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC details - Document of photo card'
set @tempdescription = 'If claimant modifies the DOC details - Document of photo card'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC details - Document of photo card'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Document_validate'
set @tempcondition = 2
set @tempvalue1 = '6dd910d7-01df-4a42-b0a8-ddc8ca0bbb0e'
set @tempvalue2 = ''
set @criteriaFieldID = '7fef1ddf-73d7-47d0-96ca-5b8dd3f62e6c'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Document of photo card'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Photocard Back to duplicate attribute'
set @tempdescription = 'Copy Photocard Back to duplicate attribute'
set @formula = '("[6dd910d7-01df-4a42-b0a8-ddc8ca0bbb0e]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Document_validate'
set @criteriaFieldID = '7fef1ddf-73d7-47d0-96ca-5b8dd3f62e6c'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC details - Document of photo card'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Invalidated'
set @tempdescription = 'Set Review to Invalidated'
set @formula = @tempInvalidatedValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the Driving Licence details'
set @tempdescription = 'If claimant modifies the Driving Licence details'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the Driving Licence details'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @tempcondition = 1
set @tempvalue1 = @tempInvalidatedValue
set @tempvalue2 = ''
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the Driving Licence details'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Invalidated review'
set @tempdescription = 'Invalidated review'
set @tempemailtemplatename = 'Send to an approver when claimant modifies a driving licence document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the Driving Licence details'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If the user is claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow After Values are assigned'
set @tempdescription = 'Finish Workflow After Values are assigned'
set @relatedStepDescription = 'If claimant modifies the Driving Licence details'
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
set @formula = 'IF("[e3d6bb3b-dbcd-4bf9-b5bb-5dc3c2bfa5ae]">" ",1,0)'
set @tablename = 'Driving Licence'
set @fieldname = 'ApproverCheck'
set @criteriaFieldID = 'a7043be5-1abe-44a9-889e-a7bc10587403'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Date of issue to duplicate attribute'
set @tempdescription = 'Copy Date of issue to duplicate attribute'
set @formula = '("[23bae96e-bdcd-455e-bee2-e0d4ba4fd53d]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Date of issue_validate'
set @criteriaFieldID = '73955a17-ea39-40e2-ab65-4b835a1f5016'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Issue Number to duplicate attribute'
set @tempdescription = 'Copy Issue Number to duplicate attribute'
set @formula = '("[f55790e3-812a-47f5-8f6d-8103c82ec286]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Issue Number_validate'
set @criteriaFieldID = '898aec93-3671-4c46-aca8-3cb7f0541fec'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Issuing authority to duplicate attribute'
set @tempdescription = 'Copy Issuing authority to duplicate attribute'
set @formula = '("[1e5a0ad0-d5a3-4304-8381-b937bed9bb8b]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Issuing authority_validate'
set @criteriaFieldID = '0eb394b1-52fe-4639-b1fe-14920fa8e50f'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Licence numbery to duplicate attribute'
set @tempdescription = 'Copy Licence numbery to duplicate attribute'
set @formula = '("[10dfd838-201e-4258-a6b9-259bfc137062]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Licence number_validate'
set @criteriaFieldID = '87c3c54e-6f55-4786-b180-10705edffa20'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Check Code to duplicate attribute'
set @tempdescription = 'Copy Check Code to duplicate attribute'
set @formula = '("[c6567181-9f8e-49c7-8063-4e73ce347564]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Check Code_validate'
set @criteriaFieldID = '4d302354-9ba4-48a6-8512-de9054ccc9b2'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Check Code date to duplicate attribute'
set @tempdescription = 'Copy Check Code date to duplicate attribute'
set @formula = '("[3e9ac071-04bf-44e2-96c4-c3c417e2a88b]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Check Code Expiry Date_validate'
set @criteriaFieldID = 'ef14b850-0f39-4244-a23b-887df2cd3cdc'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a new Photo Licence'
set @tempdescription = 'If its a new Photo Licence'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a new Photo Licence'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Licence type'
set @tempcondition = 1
set @tempvalue1 = @tempPhotoCardValue
set @tempvalue2 = ''
set @criteriaFieldID = '63a7f8ad-4391-4b86-839d-798389ceba1b'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a new Photo Licence'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Photocard Front to duplicate attribute'
set @tempdescription = 'Copy Photocard Front to duplicate attribute'
set @formula = '("[3210038d-7db4-45e4-b502-5587e597eeb5]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Photocard Front_validate'
set @criteriaFieldID = '67c50884-4355-49c2-904d-544b20a4a590'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a new Photo Licence'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Photocard Back to duplicate attribute'
set @tempdescription = 'Copy Photocard Back to duplicate attribute'
set @formula = '("[18a3c39e-8c83-4bda-b224-58648a4262ea]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Photocard Back_validate'
set @criteriaFieldID = 'ccd9f9d7-276b-466b-9ceb-0f313a96ab90'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a new Photo Licence'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Expiry date to duplicate attribute'
set @tempdescription = 'Copy Expiry date to duplicate attribute'
set @formula = '("[07c09062-68c2-4e1a-863a-c8fa94911288]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Expiry date_validate'
set @criteriaFieldID = 'efc2ac8f-5746-410d-959b-4694b03cccff'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If its a new Paper Licence'
set @tempdescription = 'If its a new Paper Licence'
set @relatedStepDescription = 'If its a new Photo Licence'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 8, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If its a new Paper Licence'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Licence type'
set @tempcondition = 1
set @tempvalue1 = @tempPaperValue
set @tempvalue2 = ''
set @criteriaFieldID = '63a7f8ad-4391-4b86-839d-798389ceba1b'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If its a new Paper Licence'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Copy Document to duplicate attribute'
set @tempdescription = 'Copy Document to duplicate attribute'
set @formula = '("[6dd910d7-01df-4a42-b0a8-ddc8ca0bbb0e]")'
set @tablename = 'Driving Licence'
set @fieldname = 'Document_validate'
set @criteriaFieldID = '7fef1ddf-73d7-47d0-96ca-5b8dd3f62e6c'
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
set @tablename = 'Driving Licence'
set @fieldname = 'Reviewed by'
set @tempcondition = 2
set @tempvalue1 = 'DDC48A05-0D47-4A05-A3D9-488FE15E5AE5'
set @tempvalue2 = ''
set @criteriaFieldID = 'e3d6bb3b-dbcd-4bf9-b5bb-5dc3c2bfa5ae'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Check if the user is reviewer whos editing the form'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Send Email to Notify Reviewer that new driving licence document has been submitted by a claimant that requires review'
set @tempdescription = 'Send Email to Notify Reviewer that new driving licence document has been submitted by a claimant that requires review'
set @tempemailtemplatename = 'Send to an approver when claimant adds a new driving licence document.'
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
set @tablename = 'Driving Licence'
set @fieldname = 'ApproverCheck'
set @tempcondition = 1
set @tempvalue1 = '0'
set @tempvalue2 = ''
set @criteriaFieldID = 'a7043be5-1abe-44a9-889e-a7bc10587403'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Block Approver From Getting Mails'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Awaiting Review'
set @tempdescription = 'Set Review to Awaiting Review'
set @formula = @tempAwaitingReviewValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'Block Approver From Getting Mails'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Send Email to Notify Reviewer that new driving licence document has been submitted by a claimant that requires review'
set @tempdescription = 'Send Email to Notify Reviewer that new driving licence document has been submitted by a claimant that requires review'
set @tempemailtemplatename = 'Send to an approver when claimant adds a new driving licence document.'
set @tempemailtemplateid = (select emailtemplateid from emailtemplates where templatename = @tempemailtemplatename)
EXEC [dbo].[saveWorkflowStepEmailTemplate] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @emailTemplateID = @tempemailtemplateid, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step If claimant modifies the DOC Driving Licence details - when the status is invalidated'
set @tempdescription = 'If claimant modifies the DOC Driving Licence details - when the status is invalidated'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 3, @relatedStepID = null, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''If claimant modifies the DOC Driving Licence details - when the status is invalidated'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @tempcondition = 2
set @tempvalue1 = @tempInvalidatedValue
set @tempvalue2 = ''
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC [dbo].[saveWorkflowStepCriteria] @workflowid = @tempworkflowid, @workflowStepID = @tempWorkflowStepID, @fieldID = @criteriaFieldID, @operator = @tempcondition, @valueOne = @tempvalue1, @valueTwo = @tempValue2, @runtime = 0, @updateCriteria = 0, @replaceElements = 0, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC Driving Licence details - when the status is invalidated'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Set Review to Awaiting Review'
set @tempdescription = 'Set Review to Awaiting Review'
set @formula = @tempAwaitingReviewValue
set @tablename = 'Driving Licence'
set @fieldname = 'Status'
set @criteriaFieldID = '8285d4e6-0b75-4004-bbd8-7a1c2670197a'
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowCreateDynamicValueStep] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @CUemployeeID = @employeeid, @CUdelegateID = null
EXEC [dbo].[saveWorkflowStepCreateDynamicValue] @workflowStepID = @tempWorkflowStepID, @dynamicValueFormula = @formula, @fieldID = @criteriaFieldID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If claimant modifies the DOC Driving Licence details - when the status is invalidated'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow'
EXEC [dbo].[saveWorkflowStepFinishWorkflow] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @CUemployeeID = @employeeid, @CUdelegateID = null
set @parentdescription = 'If there is a review request from Claimant'
set @tempparentstepID = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and description = @parentdescription)
print 'Creating workflow step Finish Workflow if there is a review request from approver'
set @tempdescription = 'Finish Workflow if there is a review request from approver'
set @relatedStepDescription = 'If claimant modifies the DOC Driving Licence details - when the status is invalidated'
set @relatedStep = (select workflowstepid from workflowsteps where workflowid = @tempworkflowID and [description] = @relatedStepDescription)
EXEC @tempWorkflowStepID = [dbo].[saveWorkflowStepCheckCondition] @workflowid = @tempworkflowid, @parentStepID = @tempparentstepID, @description = @tempdescription, @stepAction = 10, @relatedStepID = @relatedStep, @CUemployeeID = @employeeid, @CUdelegateID = null
set @count = (select count(*) from #stepids where [description] = @tempdescription)
if @count = 0
insert into #stepids (description, id) values (@tempdescription, @tempWorkflowStepID)
else
begin
print 'Description ''Finish Workflow if there is a review request from approver'' has been used more than once. Descriptions must be unique.'
rollback transaction
return
end
set @parentdescription = 'Finish Workflow if there is a review request from approver'
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
SET baseTableID = 'F066EF8F-705F-4CD7-8DD7-FDEFBF8F3821'
WHERE workflowName = 'Notify Claimant and Approver On Duty Of Care Driving License Review Status'
GO
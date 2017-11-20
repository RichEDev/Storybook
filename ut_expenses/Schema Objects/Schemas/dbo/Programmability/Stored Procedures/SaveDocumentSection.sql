
CREATE procedure [dbo].[SaveDocumentSection]
@mergeProjectId int = 0,
@documentPartName nvarchar(255),
@documentData varbinary(max),
@firstPass bit = 0
as

if @firstPass = 1
delete from dbo.DocumentTemplatePartsData where mergeProjectId = @mergeProjectId;

INSERT INTO [dbo].[DocumentTemplatePartsData]
	([MergeProjectId]
	,[GroupingId]
	,[DocumentData]
	,[DocumentPartName])
    VALUES
	(@mergeProjectId
	,null
	,@documentData
	,@documentPartName)
	
return SCOPE_IDENTITY();
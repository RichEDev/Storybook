
CREATE PROCEDURE [dbo].[TorchGetDocumentSectionsForMergeProject]
@mergeProjectId int = 0
AS

BEGIN
    DECLARE @autoPosition int = 0;

    set @autoPosition = (select sequenceorder from dbo.DocumentPredefinedSections where MergeProjectId = @mergeProjectId and SectionName = '**AUTO**');

    declare @DocumentData table
    (
	   SequenceOrder int IDENTITY(1,1),
	   DocumentPartName nvarchar(255),
	   DocumentData varbinary(max)
    );

    insert @DocumentData
	   select d.DocumentPartName, d.DocumentData from DocumentTemplatePartsData d 
	   inner join DocumentPredefinedSections p on d.DocumentPartName = p.SectionName and d.MergeProjectId = p.MergeProjectId
	   where p.SequenceOrder < @autoPosition;
        
    insert @DocumentData
	   select DocumentPartName, DocumentData from DocumentTemplatePartsData where DocumentPartName not in 
		  (select SectionName from DocumentPredefinedSections where MergeProjectId = @mergeProjectId)
	   order by DocumentPartId;
        
    insert @DocumentData
	   select d.DocumentPartName, d.DocumentData from DocumentTemplatePartsData d 
	   inner join DocumentPredefinedSections p on d.DocumentPartName = p.SectionName and d.MergeProjectId = p.MergeProjectId
	   where p.SequenceOrder > @autoPosition;
               
    select * from @DocumentData

END
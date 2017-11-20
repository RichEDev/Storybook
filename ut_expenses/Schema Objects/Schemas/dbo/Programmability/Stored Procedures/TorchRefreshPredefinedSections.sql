
CREATE PROCEDURE [dbo].[TorchRefreshPredefinedSections]

AS
BEGIN
delete from [dbo].[DocumentPredefinedSections]
 
DECLARE theCursor Cursor 
FOR 
Select distinct mergeprojectid, groupingid from [dbo].[DocumentGroupingConfigurations]

Open theCursor

DECLARE @mergeProjectId int, @groupingId int

Fetch NEXT FROM theCursor INTO  @mergeProjectId, @groupingId
print @@FETCH_STATUS
While (@@FETCH_STATUS <> -1)
BEGIN
    IF (@@FETCH_STATUS <> -2)
	   BEGIN

		  INSERT [dbo].[DocumentPredefinedSections]           
			 select  @mergeProjectId, @groupingId, 'Header', 1
		  INSERT [dbo].[DocumentPredefinedSections]           
			 select  @mergeProjectId, @groupingId, 'Index', 2
		  INSERT [dbo].[DocumentPredefinedSections]           
			 select  @mergeProjectId, @groupingId, 'Introduction', 3
		  INSERT [dbo].[DocumentPredefinedSections]           
			 select  @mergeProjectId, @groupingId, '**AUTO**', 4
		  INSERT [dbo].[DocumentPredefinedSections]           
			 select  @mergeProjectId, @groupingId, 'PreCompletionChecklist', 5
		  INSERT [dbo].[DocumentPredefinedSections]           
			 select  @mergeProjectId, @groupingId, 'QualificationsAndAssumptions', 6			 			 			 			 			 			 			 			 
	   END


FETCH NEXT FROM theCursor INTO @mergeProjectId, @groupingId 
END
CLOSE theCursor
DEALLOCATE theCursor

END
CREATE procedure [dbo].[DeleteDocumentSortingConfigurationColumnsByConfigurationId] 

@DocumentGroupingConfigurationId int

AS

BEGIN

      DELETE FROM dbo.DocumentSortingConfigurationColumns
      WHERE  GroupingId = @DocumentGroupingConfigurationId;
      
END
GO

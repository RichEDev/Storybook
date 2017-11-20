CREATE PROCEDURE [dbo].[DeleteDocumentReportSortingConfigurationColumnsByConfigurationId] 

@DocumentGroupingConfigurationId INT

AS

BEGIN

      DELETE FROM dbo.DocumentReportSortingConfigurations
      WHERE  GroupingId = @DocumentGroupingConfigurationId;
      
END
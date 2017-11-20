CREATE PROCEDURE [dbo].[UpdateReportSortingConfiguration] 
     
 @ReportSortingColumns dbo.ReportSortingColumn READONLY,
 @ReportSortingConfigurationId INT,
 @ReportName NVARCHAR(255)
 
AS

DECLARE @DocumentSortingReportId INT 

BEGIN
    
    INSERT INTO DocumentSortingReports (ReportSortingConfigurationId,ReportName)
    VALUES (@ReportSortingConfigurationId, @ReportName)
        
    SET @DocumentSortingReportId = SCOPE_IDENTITY();
  
 END  
 
 BEGIN
 
    INSERT INTO dbo.DocumentSortingReportColumns(DocumentSortingReportId, SequenceOrder,ColumnName, DocumentSortTypeId)
    SELECT @DocumentSortingReportId,Sequence, ColumnName, sortingorder FROM @ReportSortingColumns
    
 END
   
   RETURN 1
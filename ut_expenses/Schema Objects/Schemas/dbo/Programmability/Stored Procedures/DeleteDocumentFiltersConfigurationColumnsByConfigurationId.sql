Create procedure DeleteDocumentFiltersConfigurationColumnsByConfigurationId 

@DocumentGroupingConfigurationId int

AS

BEGIN
      DELETE FROM dbo.DocumentFiltersConfigurationColumns
      WHERE  GroupingId = @DocumentGroupingConfigurationId;
      
END

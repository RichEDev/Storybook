
CREATE PROCEDURE [dbo].[SaveReportSortingConfiguration] @MergeProjectId                  INT,
                                                        @DocumentGroupingConfigurationId INT
                                                   
AS
   
    DECLARE @Id INT = 0

    BEGIN
    
		  INSERT INTO dbo.DocumentReportSortingConfigurations
					  (GroupingId,
					   MergeProjectId)
		  VALUES      (@DocumentGroupingConfigurationId,
						@MergeProjectId)

		  SET @Id = SCOPE_IDENTITY();

		  RETURN @Id
 
	 END 

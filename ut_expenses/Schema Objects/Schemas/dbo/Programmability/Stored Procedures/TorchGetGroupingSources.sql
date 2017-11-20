 CREATE PROCEDURE [dbo].[TorchGetGroupingSources] 
 @MergeProjectId          INT = 0,
 @GroupingConfigurationId INT = 0
AS
    SELECT reportname    
    FROM   [dbo].[TorchGroupingSources]
    WHERE  (TorchGroupingSources.mergeprojectid = @MergeProjectId)
           AND (TorchGroupingSources.groupingid = @GroupingConfigurationId )  



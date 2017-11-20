Create PROCEDURE [dbo].[TorchGetGroupingConfigurationLabel] 

@MergeProjectId          INT = 0,
@GroupingConfigurationId INT = 0

AS

  SELECT label , [description]   
    FROM   dbo.DocumentGroupingConfigurations
    WHERE  (Mergeprojectid = @MergeProjectId)
            AND (Groupingid = @GroupingConfigurationId )
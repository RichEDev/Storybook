CREATE PROCEDURE [dbo].[TorchGetGroupingColumnsById]
 @groupingConfigurationId INT = 0
AS
    SELECT [groupingcolumn]
    FROM   [dbo].[documentgroupingconfigurationcolumns]
    WHERE  groupingid = @groupingConfigurationId
    ORDER BY SequenceOrder


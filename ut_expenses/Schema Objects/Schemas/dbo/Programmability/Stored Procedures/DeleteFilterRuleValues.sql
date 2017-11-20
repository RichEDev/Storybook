CREATE PROCEDURE [dbo].[DeleteFilterRuleValues] 

@filterId INT

AS

DELETE 
FROM filter_rule_values
WHERE filterid = @filterid

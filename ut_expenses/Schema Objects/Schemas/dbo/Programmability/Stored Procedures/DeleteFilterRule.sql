CREATE PROCEDURE [dbo].[DeleteFilterRule] 

@filterId INT

AS

DELETE 
FROM filter_rule_values
WHERE filterid = @filterid

DELETE
FROM filter_rules
WHERE filterid = @filterId

GO

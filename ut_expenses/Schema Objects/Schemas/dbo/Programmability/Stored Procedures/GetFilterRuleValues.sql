CREATE PROCEDURE [dbo].[GetFilterRuleValues]
AS
SELECT filterruleid
	,parentid
	,childid
	,filterid
	,createdby
	,createdon
FROM filter_rule_values
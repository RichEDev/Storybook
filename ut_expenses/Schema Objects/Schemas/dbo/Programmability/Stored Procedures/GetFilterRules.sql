CREATE PROCEDURE GetFilterRules
AS
SELECT filterid
	,parent
	,child
	,paruserdefineid
	,childuserdefineid
	,[enabled]
	,createdon
	,createdby
FROM dbo.filter_rules

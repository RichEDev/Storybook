CREATE PROCEDURE [dbo].[SaveFilterRuleValue] 

 @parentId INT
,@childId INT
,@filterId INT
,@createdBy INT
,@identity INT OUT

AS

INSERT INTO filter_rule_values (
	parentid
	,childid
	,filterid
	,createdon
	,createdby
	)
VALUES (
	@parentId
	,@childId
	,@filterId
	,GetDate()
	,@createdBy
	);

SET @identity = @@identity

RETURN @identity
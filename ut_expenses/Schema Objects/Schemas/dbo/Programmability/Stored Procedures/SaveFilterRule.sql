CREATE PROCEDURE SaveFilterRule 
     @parent TINYINT
	,@child TINYINT
	,@parentUserDefineId INT
	,@childUserDefineId INT
	,@enabled BIT
	,@createdBy INT
	,@identity INT OUT
AS
INSERT INTO filter_rules (
	 parent
	,child
	,paruserdefineid
	,childuserdefineid
	,[enabled]
	,createdon
	,createdby
	)
VALUES (
	@parent
	,@child
	,@parentUserDefineId
	,@childUserDefineId
	,@enabled
	,GetDate()
	,@createdby
	);

SET @identity = @@identity 

RETURN @identity


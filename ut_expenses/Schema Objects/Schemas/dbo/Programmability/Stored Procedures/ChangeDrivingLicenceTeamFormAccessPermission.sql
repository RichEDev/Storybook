CREATE  PROCEDURE [dbo].[ChangeDrivingLicenceTeamFormAccessPermission] @NewValue NVARCHAR(150)
AS
DECLARE @AttributeID INT
DECLARE @ReviewerForms TABLE (FormId INT)
DECLARE @FilterCount INT = 0

SELECT @AttributeID = attributeid
FROM dbo.customEntityAttributes
WHERE dbo.customEntityAttributes.SystemCustomEntityAttributeID	 = '91596ACE-E16C-4DB3-9E02-776785EB11CF'

INSERT INTO @ReviewerForms
SELECT formid
FROM [dbo].[customEntityForms]
WHERE dbo.customEntityForms.SystemCustomEntityFormId IN
	(
	'C50DE908-69B3-4F7B-B6B6-32C11C42D24D',
	'F0CBBA4B-B959-43B8-99B3-603ABB3FB2E3',
	'12C9B402-00CD-4ABD-A973-90E9050164D5',
	'AED79586-429C-4AA9-BCC2-ABB487D774DE'
	)
GROUP BY dbo.customEntityForms.formid

SELECT @FilterCount = count(1)
FROM fieldFilters
INNER JOIN @ReviewerForms rf ON rf.formid = fieldFilters.formid
WHERE fieldFilters.fieldid = '1C45B860-DDAA-47DA-9EEC-981F59CCE795'
	AND fieldFilters.value = '@MY_HIERARCHY'

DELETE fieldFilters
FROM fieldFilters
INNER JOIN @ReviewerForms rf ON rf.formid = fieldFilters.formid
WHERE fieldFilters.fieldid = '1C45B860-DDAA-47DA-9EEC-981F59CCE795'
	AND fieldFilters.value = '@MY_HIERARCHY'

IF @NewValue = '@MY_HIERARCHY'
	AND @FilterCount = 0
BEGIN
	DECLARE @joinviaid INT = NULL

	SELECT @joinviaid = [joinviaid]
	FROM [dbo].[joinVia]
	WHERE dbo.joinVia.joinViaDescription	='Line Manager'

	INSERT INTO fieldFilters (
		[formid]
		,[fieldid]
		,[condition]
		,[value]
		,[order]
		,[joinViaID]
		,[valueTwo]
		,[AttributeID]
		)
	SELECT rf.formid
		,'1C45B860-DDAA-47DA-9EEC-981F59CCE795'
		,255 --equals
		,@NewValue
		,(SELECT isnull(1 + MAX([Order]),0) FROM fieldFilters WHERE formid = rf.formid) --order
		,@joinviaid --joinviaid
		,''
		,@AttributeID
	FROM @ReviewerForms rf
END

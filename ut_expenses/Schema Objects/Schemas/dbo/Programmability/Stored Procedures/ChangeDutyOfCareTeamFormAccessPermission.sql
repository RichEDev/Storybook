CREATE PROCEDURE [dbo].[ChangeDutyOfCareTeamFormAccessPermission] @NewValue NVARCHAR(150)
AS
DECLARE @AttributeID INT
DECLARE @ReviewerForms TABLE (FormId INT)
DECLARE @FilterCount INT = 0
DECLARE @UsernameFilterId UNIQUEIDENTIFIER = '1C45B860-DDAA-47DA-9EEC-981F59CCE795'

SELECT @AttributeID = attributeid
FROM dbo.customEntityAttributes
WHERE fieldid = 'EFBD0C7F-0ECC-406F-BE6C-ED9978604439'

INSERT INTO @ReviewerForms
SELECT formid
FROM [dbo].[customEntityForms]
WHERE dbo.customEntityForms.SystemCustomEntityFormId IN
(
	'7A3A9523-0F15-4118-ACE2-D2DE8E230B22',
	'5B8AF04E-A1B1-4822-ACEB-604CA58C93CC',
	'D0EBB27A-2593-464F-A5C0-DE592CDB56D2',
	'37D764E4-9C84-4863-B244-20E0CD35D691',
	'8CCC36BB-83D6-4EA7-88D8-41F6DBC1D896'
)
GROUP BY dbo.customEntityForms.formid

SELECT @FilterCount = count(1)
FROM fieldFilters
INNER JOIN @ReviewerForms rf ON rf.formid = fieldFilters.formid
WHERE fieldFilters.fieldid = @UsernameFilterId
	AND fieldFilters.value = '@MY_HIERARCHY'

DELETE fieldFilters
FROM fieldFilters
INNER JOIN @ReviewerForms rf ON rf.formid = fieldFilters.formid
WHERE fieldFilters.fieldid = @UsernameFilterId
	AND fieldFilters.value = '@MY_HIERARCHY'

IF @NewValue = '@MY_HIERARCHY'
	AND @FilterCount = 0
BEGIN
	DECLARE @joinviaid INT = NULL

	SELECT @joinviaid = [joinviaid]
	FROM [dbo].[joinVia]
	WHERE dbo.joinVia.joinViaDescription	= 'Employee ID: Line Manager'

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
		,@UsernameFilterId
		,255 --equals
		,@NewValue
		,(SELECT isnull(1 + MAX([Order]),0) FROM fieldFilters WHERE formid = rf.formid)
		,@joinviaid --joinviaid
		,''
		,@AttributeID
	FROM @ReviewerForms rf
END

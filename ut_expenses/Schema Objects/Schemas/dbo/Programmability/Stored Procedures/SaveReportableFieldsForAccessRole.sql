CREATE PROCEDURE [dbo].[SaveReportableFieldsForAccessRole] @CUemployeeID INT
	,@CUDelegateID INT
	,@AccessRoleId INT
	,@excludedField GuidPk readonly
	,@includedFields GuidPk readonly
AS
DECLARE @FieldId UNIQUEIDENTIFIER
DECLARE @Title NVARCHAR(100)
DECLARE @recordTitle NVARCHAR(50)
DECLARE @roleName NVARCHAR(200) = (
		SELECT rolename
		FROM accessRoles
		WHERE roleID = @accessRoleID
		);
DECLARE @Id INT = 1
DECLARE @Counter INT

SELECT ROW_NUMBER() OVER (
		ORDER BY id
		) AS SlNo
	,id
INTO #includedFieldsTable
FROM @includedFields

SELECT ROW_NUMBER() OVER (
		ORDER BY id
		) AS SlNo
	,id
INTO #excludedFieldsTable
FROM @excludedField

SELECT @Counter = Count(*)
FROM #includedFieldsTable

WHILE @Counter + 1 > @Id
BEGIN
	SELECT @FieldId = id
	FROM #includedFieldsTable
	WHERE SlNo = @Id

	DELETE
	FROM #includedFieldsTable
	WHERE SlNo = @Id

	SET @Id = @Id + 1

	IF NOT EXISTS (
			SELECT *
			FROM reportingfields
			WHERE accessroleid = @AccessRoleId
				AND fieldId = @FieldId
			)
	BEGIN
		SELECT @recordTitle = tablename
		FROM tables
		WHERE tableid = (
				SELECT tableid
				FROM fields
				WHERE fieldid = @FieldId
				)

		SET @title = @roleName + ' - ' + @recordTitle;

		INSERT INTO reportingfields (
			[AccessRoleID]
			,[FieldID]
			,[CreatedOn]
			)
		VALUES (
			@AccessRoleId
			,@FieldId
			,GETDATE()
			)

		IF EXISTS (
				SELECT *
				FROM accessroles
				WHERE roleid = @AccessRoleId
					AND Modifiedon IS NOT NULL
				)
			EXEC addUpdateEntryToAuditLog @CUemployeeID
				,@CUdelegateID
				,6
				,@accessRoleID
				,@FieldId
				,1
				,0
				,@title
				,NULL
	END
END

SET @Id = 1

SELECT @Counter = Count(*)
FROM #excludedFieldsTable

WHILE @Counter + 1 > @Id
BEGIN
	SELECT @FieldId = id
	FROM #excludedFieldsTable
	WHERE SlNo = @Id

	DELETE
	FROM #excludedFieldsTable
	WHERE SlNo = @Id

	SET @Id = @Id + 1

	IF EXISTS (
			SELECT *
			FROM reportingfields
			WHERE accessroleid = @AccessRoleId
				AND fieldId = @FieldId
			)
	BEGIN
		SELECT @recordTitle = tablename
		FROM tables
		WHERE tableid = (
				SELECT tableid
				FROM fields
				WHERE fieldid = @FieldId
				)

		SET @title = @roleName + ' - ' + @recordTitle;

		DELETE
		FROM reportingfields
		WHERE accessroleid = @AccessRoleId
			AND fieldId = @FieldId

		IF EXISTS (
				SELECT *
				FROM accessroles
				WHERE roleid = @AccessRoleId
					AND Modifiedon IS NOT NULL
				)
			EXEC addUpdateEntryToAuditLog @CUemployeeID
				,@CUdelegateID
				,6
				,@accessRoleID
				,@FieldId
				,1
				,0
				,@title
				,NULL
	END
END
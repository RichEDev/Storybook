CREATE PROCEDURE [dbo].[GetReportingFieldsByTableId]
	-- Add the parameters for the stored procedure here
	@TableName UNIQUEIDENTIFIER
	,@AccessRoleID INT
AS
BEGIN
	DECLARE @USERDEFINEDTABLE UNIQUEIDENTIFIER = (
			SELECT userdefined_table
			FROM tables
			WHERE tableid = @TableName
			)

	IF @USERDEFINEDTABLE = NULL
	BEGIN
		SELECT f.fieldid
			,f.description AS [Field Name]
			,f.comment AS Description
			,@AccessRoleID AS roleid
			,CASE 
				WHEN (
						SELECT AccessRoleID
						FROM reportingfields r
						WHERE f.fieldid = r.FieldID
							AND r.AccessRoleID = @AccessRoleID
						) IS NOT NULL
					THEN 1
				ELSE NULL
				END AS exclusiontype
		FROM fields f
		WHERE tableid = @TableName
		ORDER BY [Field Name]
	END
	ELSE
	BEGIN
		DECLARE @ForeignKey UNIQUEIDENTIFIER = (
				SELECT fieldid
				FROM Fields
				WHERE tableid = @USERDEFINEDTABLE
					AND IsForeignKey = 1 AND idfield = 1
				)

		SELECT f.fieldid
			,f.description AS [Field Name]
			,f.comment AS Description
			,@AccessRoleID AS roleid
			,CASE 
				WHEN (
						SELECT AccessRoleID
						FROM reportingfields r
						WHERE f.fieldid = r.FieldID
							AND r.AccessRoleID = @AccessRoleID
						) IS NOT NULL
					THEN 1
				ELSE NULL
				END AS exclusiontype
		FROM fields f
		WHERE tableid = @TableName
		
		UNION ALL
		
		SELECT f.fieldid
			,f.description AS [Field Name]
			,f.comment AS Description
			,@AccessRoleID AS roleid
			,CASE 
				WHEN (
						SELECT AccessRoleID
						FROM reportingfields r
						WHERE f.fieldid = r.FieldID
							AND r.AccessRoleID = @AccessRoleID
						) IS NOT NULL
					THEN 1
				ELSE NULL
				END AS exclusiontype
		FROM fields f
		WHERE tableid = @USERDEFINEDTABLE
			AND f.fieldid <> @ForeignKey
		ORDER BY [Field Name]
	END
END
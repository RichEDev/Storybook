
CREATE PROCEDURE [dbo].[ApiBatchSaveEsrTrust] @list ApiBatchSaveEsrTrustType READONLY
AS
BEGIN
	DECLARE @index INT
	DECLARE @count INT
	DECLARE @trustID INT
	DECLARE @trustVPD NVARCHAR(3)
	DECLARE @periodType NVARCHAR(1)
	DECLARE @periodRun NVARCHAR(1)
	DECLARE @runSequenceNumber INT
	DECLARE @ftpAddress NVARCHAR(100)
	DECLARE @ftpUsername NVARCHAR(100)
	DECLARE @ftpPassword NVARCHAR(100)
	DECLARE @archived BIT
	DECLARE @createdOn DATETIME
	DECLARE @modifiedOn DATETIME
	DECLARE @trustName NVARCHAR(150)
	DECLARE @delimiterCharacter NVARCHAR(5)
	DECLARE @ESRVersionNumber TINYINT
	DECLARE @currentOutboundSequence INT
	DECLARE @tmp TABLE (
		tmpID BIGINT
		,trustId BIGINT
		)

	INSERT @tmp
	SELECT ROW_NUMBER() OVER (
			ORDER BY trustId
			)
		,trustId
	FROM @list

	SELECT @count = count(*)
	FROM @tmp

	SET @index = 1

	WHILE @index <= @count
	BEGIN
		SET @trustId = (
				SELECT TOP 1 trustId
				FROM @tmp
				WHERE tmpID = @index
				);

		SELECT TOP 1 @trustVPD = trustVPD
			,@periodType = periodType
			,@periodRun = periodRun
			,@runSequenceNumber = runSequenceNumber
			,@ftpAddress = ftpAddress
			,@ftpUsername = ftpUsername
			,@ftpPassword = ftpPassword
			,@archived = archived
			,@createdOn = createdOn
			,@modifiedOn = modifiedOn
			,@trustName = trustName
			,@delimiterCharacter = delimiterCharacter
			,@ESRVersionNumber = ESRVersionNumber
			,@currentOutboundSequence = currentOutboundSequence
		FROM @list
		WHERE trustId = @trustID

		IF @trustID = 0
		BEGIN
			INSERT INTO [dbo].[esrTrusts] (
				[trustVPD]
				,[periodType]
				,[periodRun]
				,[runSequenceNumber]
				,[ftpAddress]
				,[ftpUsername]
				,[ftpPassword]
				,[archived]
				,[createdOn]
				,[modifiedOn]
				,[trustName]
				,[delimiterCharacter]
				,[ESRVersionNumber]
				,[currentOutboundSequence]
				)
			VALUES (
				@trustVPD
				,@periodType
				,@periodRun
				,@runSequenceNumber
				,@ftpAddress
				,@ftpUsername
				,@ftpPassword
				,@archived
				,@createdOn
				,@modifiedOn
				,@trustName
				,@delimiterCharacter
				,@ESRVersionNumber
				,@currentOutboundSequence
				)

			SET @trustID = scope_identity();
		END
		ELSE
		BEGIN
			UPDATE [dbo].[esrTrusts]
			SET [trustVPD] = @trustVPD
				,[periodType] = @periodType
				,[periodRun] = @periodRun
				,[runSequenceNumber] = @runSequenceNumber
				,[ftpAddress] = @ftpAddress
				,[ftpUsername] = @ftpUsername
				,[ftpPassword] = @ftpPassword
				,[archived] = @archived
				,[createdOn] = @createdOn
				,[modifiedOn] = @modifiedOn
				,[trustName] = @trustName
				,[delimiterCharacter] = @delimiterCharacter
				,[ESRVersionNumber] = @ESRVersionNumber
				,[currentOutboundSequence] = @currentOutboundSequence
			WHERE trustID = @trustID
		END

		SET @index = @index + 1
	END

	RETURN 0;
END
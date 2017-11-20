
CREATE PROCEDURE [dbo].[saveESRTrust]
	@trustID int,
	@trustName nvarchar(150),
	@trustVPD nvarchar(3),
	@periodType nvarchar(1),
	@periodRun nvarchar(1),
	@ftpAddress nvarchar(100),
	@ftpUsername nvarchar(100),
	@ftpPassword nvarchar(100),
	@runSequenceNumber INT,
	@employeeID INT,
	@delegateID INT,
	@delimiterCharacter nvarchar(5)
AS 
	DECLARE @count INT;

	IF (@trustID = 0)
		BEGIN
			SET @count = (SELECT COUNT(*) FROM esrTrusts WHERE trustName = @trustName);
		
			IF @count > 0
				RETURN -1;

			INSERT INTO esrTrusts (trustName, trustVPD, periodType, periodRun, ftpAddress, ftpUsername, ftpPassword, createdOn, runSequenceNumber, delimiterCharacter) VALUES (@trustName, @trustVPD, @periodType, @periodRun, @ftpAddress, @ftpUsername, @ftpPassword, getutcdate(), @runSequenceNumber, @delimiterCharacter);
			SET @trustID = scope_identity();

			exec addInsertEntryToAuditLog @employeeID, @delegateID, 27, @trustID, @trustName, null;
		END
	ELSE
		BEGIN

			SET @count = (SELECT COUNT(*) FROM esrTrusts WHERE trustName = @trustName AND trustID <> @trustID);
			
			IF @count > 0
				RETURN -1;

			declare @oldtrustName nvarchar(150);
			declare @oldtrustVPD nvarchar(3);
			declare @oldperiodType nvarchar(1);
			declare @oldperiodRun nvarchar(1);
			declare @oldftpAddress nvarchar(100);
			declare @oldftpUsername nvarchar(100);
			declare @oldftpPassword nvarchar(100);
			declare @oldRunSequence int;
			declare @oldDelimiterCharacter nvarchar(5);
			select @oldtrustName = trustName, @oldtrustVPD = trustVPD, @oldperiodType = periodType, @oldperiodRun = periodRun, @oldftpAddress = ftpAddress, @oldftpUsername = ftpUsername, @oldftpPassword = ftpPassword, @oldRunSequence = runSequenceNumber, @oldDelimiterCharacter = delimiterCharacter from esrTrusts WHERE trustID=@trustID;

			UPDATE esrTrusts SET trustName=@trustName, trustVPD=@trustVPD, periodType=@periodType, periodRun=@periodRun, ftpAddress=@ftpAddress, ftpUsername=@ftpUsername, ftpPassword=@ftpPassword, modifiedOn = getutcdate(), runSequenceNumber=@runSequenceNumber, delimiterCharacter = @delimiterCharacter WHERE trustID=@trustID;
			
			if @oldtrustName <> @trustName
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 27, @trustID, '82978486-6c87-4d04-afe0-f4bb1d6e737a', @oldtrustName, @trustName, @trustName, null;
			if @oldtrustVPD <> @trustVPD
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 27, @trustID, '09ada43d-b19f-41fb-a242-c30409c56c2d', @oldtrustVPD, @trustVPD, @trustName, null;
			if @oldperiodType <> @periodType
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 27, @trustID, '7f6d2e7a-555f-41df-9b66-ddf41e9b3a7e', @oldperiodType, @periodType, @trustName, null;
			if @oldperiodRun <> @periodRun
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 27, @trustID, '9deb5508-bbd1-4328-80ac-ea49d33408f3', @oldperiodRun, @periodRun, @trustName, null;
			if @oldftpAddress <> @ftpAddress
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 27, @trustID, '0e6ce9d8-412b-4b42-98b8-4b062613d78e', @oldftpAddress, @ftpAddress, @trustName, null;
			if @oldftpUsername <> @ftpUsername
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 27, @trustID, 'dcd1b7f2-57fd-4eea-b959-d997dbe33475', @oldftpUsername, @ftpUsername, @trustName, null;
			if @oldftpPassword <> @ftpPassword
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 27, @trustID, '2aa64ad3-5254-4548-ab32-2010dc500cd9', @oldftpPassword, @ftpPassword, @trustName, null;
			if @oldRunSequence <> @runSequenceNumber 
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 27, @trustID, '6A766A42-FE10-425A-A2F5-202BBB1589F5', @oldRunSequence, @runSequenceNumber, @trustName, null;
			if @oldDelimiterCharacter <> @delimiterCharacter
				exec addUpdateEntryToAuditLog @employeeID, @delegateID, 27, @trustID, 'ebba3f37-64dc-4135-b01e-328539d56eec', @oldDelimiterCharacter, @delimiterCharacter, @trustName, null;
		END
RETURN @trustID;





 

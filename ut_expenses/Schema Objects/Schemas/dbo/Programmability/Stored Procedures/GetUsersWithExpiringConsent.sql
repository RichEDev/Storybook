CREATE PROCEDURE GetUsersWithExpiringConsent
AS
BEGIN
	DECLARE @frequency INT = 0

	IF EXISTS (
			SELECT *
			FROM accountProperties
			WHERE stringKey = 'consentReminderFrequency'
			)
		SET @frequency = (
				SELECT stringValue
				FROM accountProperties
				WHERE stringKey = 'consentReminderFrequency'
				)

	IF @frequency != 0
	BEGIN
		--DECLARE @expiryDate NVARCHAR(50)
		DECLARE @originalExpiryDate NVARCHAR(50)
		DECLARE @today DATETIME = CAST(GETDATE() AS DATE);

		SELECT *
		FROM (
			SELECT employeeid
				,dbo.getDecryptedValue(drivinglicence_email) AS emailId
				,SecurityCode
				,CAST((
						SELECT DATEADD(day, - @frequency, DVLAConsentDate)
						) AS DATE) AS expiryDate
			FROM employees
			) AS TEMP
		WHERE TEMP.expiryDate = @today
	END
END
GO


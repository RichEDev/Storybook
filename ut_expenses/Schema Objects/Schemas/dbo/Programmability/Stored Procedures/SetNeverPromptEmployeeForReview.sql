CREATE PROCEDURE [dbo].[SetNeverPromptEmployeeForReview] (@EmployeeId INT)
AS
BEGIN
	DECLARE @recordId INT

	SELECT @recordId = employeeid
	FROM EmployeeMobileAppReviewPreferences
	WHERE employeeid = @EmployeeId;

	IF @recordId > 0
	BEGIN
		UPDATE EmployeeMobileAppReviewPreferences
		SET NeverPromptForReview = 1
		WHERE employeeid = @EmployeeId

		RETURN @recordId
	END
	ELSE
	BEGIN
		INSERT INTO EmployeeMobileAppReviewPreferences (
			EmployeeId
			,NeverPromptForReview
			)
		VALUES (
			@EmployeeId
			,1
			)

		RETURN SCOPE_IDENTITY();
	END
END

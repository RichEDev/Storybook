CREATE PROCEDURE [dbo].[GetLastXClaimantApprovers] @employeeId INT
	,@numberOfApprovers INT
	,@approverList IntPK READONLY
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @lastSelected INT;

	SET @lastSelected = (
			SELECT TOP 1 a.ApproverId
			FROM dbo.employees e
			INNER JOIN dbo.EmployeeSelectedClaimApprover a ON e.employeeid = a.ApproverId
			WHERE a.EmployeeId = @employeeId
				AND e.archived = 0
				AND a.ApproverId IN (
					SELECT c1
					FROM @approverList
					)
			ORDER BY a.SelectedDate DESC
			);

	IF @lastSelected IS NULL
	BEGIN
		SET @lastSelected = (
				SELECT 0
				);
	END

	SELECT @lastSelected AS LastSelectedApproverId;

	DECLARE @temp TABLE (
		ApproverId INT
		,Approver NVARCHAR(500)
		,SelectedDate DATETIME
		)

	INSERT @temp
	SELECT DISTINCT TOP (@numberOfApprovers) e.employeeid
		,e.firstname + ' ' + e.surname + ' (' + e.username + ')'
		,a.SelectedDate
	FROM dbo.employees e
	INNER JOIN dbo.EmployeeSelectedClaimApprover a ON e.employeeid = a.ApproverId
	WHERE a.EmployeeId = @employeeId
		AND e.archived = 0
		AND a.ApproverId IN (
			SELECT c1
			FROM @approverList
			)
	ORDER BY a.SelectedDate DESC;

	SELECT *
	FROM @temp
	ORDER BY Approver ASC
END


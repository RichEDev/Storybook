CREATE PROC [dbo].[GetApproversReminderList]
AS
BEGIN

DECLARE @checkReminderEnabled BIT
SELECT @checkReminderEnabled = stringValue from accountProperties where stringKey = 'EnableClaimApprovalReminders'

IF @checkReminderEnabled = 0
RETURN

DECLARE @employeeIdList TABLE
(
employeeId int 
)
DECLARE @reminderFrequency INT
DECLARE @nDaysAgo datetime

SELECT @reminderFrequency = stringValue FROM accountProperties WHERE stringKey = 'ClaimApprovalReminderFrequency'
SET @nDaysAgo = dateadd(day,- @reminderFrequency,GETDATE())

INSERT INTO @employeeIdList
SELECT DISTINCT employees.employeeid
FROM EMPLOYEES
LEFT JOIN claims_base ON claims_base.checkerid = employees.employeeid
LEFT JOIN savedexpenses ON savedexpenses.itemcheckerid = employees.employeeid
LEFT JOIN claims_base AS itemcheckerClaims ON itemcheckerClaims.claimid = savedexpenses.claimid
WHERE ISNULL(ApproverLastRemindedDate, '1900-01-01') <= @ndaysago
	AND (
			(
				claims_base.STATUS <> 4
				AND claims_base.submitted = 1
				AND claims_base.paid = 0
			) OR (
				itemcheckerClaims.STATUS <> 4
				AND itemcheckerClaims.submitted = 1
				AND itemcheckerClaims.paid = 0
			)
		)

SELECT employeeId FROM @employeeIdList

UPDATE employees SET ApproverLastRemindedDate = GETDATE() WHERE employeeid IN (SELECT employeeId FROM @employeeIdList)

INSERT INTO claimhistory (claimid, stage, comment, datestamp, employeeid, createdon, refnum) 
	SELECT DISTINCT cb.claimid, stage, employees.firstname + ' ' + employees.surname + ' has been sent an email reminder to check this claim', GETDATE(), cb.employeeid, cb.createdon, null from claims_base cb
		INNER JOIN savedexpenses se ON cb.claimId = se.claimId
		INNER JOIN  @employeeIdList CHECKERS ON SE.itemCheckerId = CHECKERS.employeeId OR cb.checkerid = CHECKERS.employeeId
		INNER JOIN employees ON CHECKERS.employeeId = employees.employeeid
		WHERE CB.paid = 0 AND CB.submitted = 1 AND CB.status <> 4

END


GO



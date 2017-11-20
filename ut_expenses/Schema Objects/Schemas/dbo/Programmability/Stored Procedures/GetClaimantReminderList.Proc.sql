CREATE PROC [dbo].[GetClaimantReminderList]
AS
BEGIN

DECLARE @checkReminderEnabled BIT
SELECT @checkReminderEnabled = stringValue from accountProperties where stringKey = 'EnableCurrentClaimsReminders'

IF @checkReminderEnabled = 0
RETURN

DECLARE @employeeIdList TABLE
(
employeeId int 
)
DECLARE @reminderFrequency INT
DECLARE @nDaysAgo datetime

SELECT @reminderFrequency = stringValue FROM accountProperties WHERE stringKey = 'CurrentClaimsReminderFrequency'
SET @nDaysAgo = cast(cast(dateadd(day,- @reminderFrequency,GETDATE()) as date) as datetime) + cast(cast('1900-01-01 23:59' as time) as datetime);

INSERT INTO @employeeIdList
SELECT DISTINCT employees.employeeid
FROM EMPLOYEES
INNER JOIN claims_base ON claims_base.employeeid = employees.employeeid
INNER JOIN savedexpenses ON savedexpenses.claimid = claims_base.claimid
WHERE (EMPLOYEES.ClaimantReminderDate IS NULL OR EMPLOYEES.ClaimantReminderDate <= @ndaysago)  AND claims_base.submitted=0

SELECT employeeId FROM @employeeIdList

UPDATE employees SET ClaimantReminderDate = GETDATE() WHERE employeeid IN (SELECT employeeId FROM @employeeIdList)

INSERT INTO claimhistory (claimid, stage, comment, datestamp, employeeid, createdon, refnum) 
 SELECT DISTINCT cb.claimid, stage,'Sent reminder email for claim submission.', GETDATE(), cb.employeeid, cb.createdon, null from claims_base cb
  INNER JOIN  @employeeIdList CHECKERS ON cb.employeeid = CHECKERS.employeeId
  INNER JOIN employees ON CHECKERS.employeeId = employees.employeeid

END
GO
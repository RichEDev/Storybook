
 CREATE PROC [dbo].[SetInitialClaimantReminderDate]
 AS
  IF EXISTS (SELECT * FROM employees WHERE ClaimantReminderDate IS NULL)
 BEGIN
 UPDATE employees SET ClaimantReminderDate=GETUTCDATE()
 END
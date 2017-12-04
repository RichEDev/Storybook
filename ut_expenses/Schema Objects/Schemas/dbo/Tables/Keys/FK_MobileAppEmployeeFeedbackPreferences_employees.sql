ALTER TABLE [dbo].[EmployeeMobileAppReviewPreferences]  WITH CHECK ADD  CONSTRAINT [FK_MobileAppEmployeeFeedbackPreferences_employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[employees] ([employeeid])

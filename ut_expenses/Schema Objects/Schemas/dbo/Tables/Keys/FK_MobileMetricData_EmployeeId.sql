ALTER TABLE [dbo].[MobileMetricData]  WITH CHECK ADD  CONSTRAINT [FK_MobileMetricData_EmployeeId] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[employees] ([employeeid])
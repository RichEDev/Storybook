ALTER TABLE [dbo].[savedexpenses]  ADD  CONSTRAINT [FK_savedExpenses_MobileMetricDeviceId] FOREIGN KEY([MobileMetricDeviceId])
REFERENCES [dbo].[MobileMetricData] ([Id])
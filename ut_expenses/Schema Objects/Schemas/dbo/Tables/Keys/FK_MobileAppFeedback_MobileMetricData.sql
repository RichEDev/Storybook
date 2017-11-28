ALTER TABLE [dbo].[MobileAppFeedback]  WITH CHECK ADD  CONSTRAINT [FK_MobileAppFeedback_MobileMetricData] FOREIGN KEY([MobileMetricId])
REFERENCES [dbo].[MobileMetricData] ([Id])
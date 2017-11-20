ALTER TABLE [dbo].[scheduled_reports_log]
    ADD CONSTRAINT [DF_scheduled_reports_log_datestamp] DEFAULT (getdate()) FOR [datestamp];


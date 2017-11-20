ALTER TABLE [dbo].[scheduled_reports]
    ADD CONSTRAINT [DF_scheduled_reports_ftpusessl] DEFAULT ((0)) FOR [ftpusessl];


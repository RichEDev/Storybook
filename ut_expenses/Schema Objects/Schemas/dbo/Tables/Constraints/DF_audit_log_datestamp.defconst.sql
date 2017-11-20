ALTER TABLE [dbo].[audit_log]
    ADD CONSTRAINT [DF_audit_log_datestamp] DEFAULT (getdate()) FOR [datestamp];


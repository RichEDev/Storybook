ALTER TABLE [dbo].[auditLog]
    ADD CONSTRAINT [DF_auditLog_datestamp] DEFAULT (getdate()) FOR [datestamp];


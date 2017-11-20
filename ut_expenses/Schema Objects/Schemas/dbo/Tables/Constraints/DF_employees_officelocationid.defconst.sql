ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_officelocationid] DEFAULT 0 FOR [officelocationid];


ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_homelocationid] DEFAULT 0 FOR [homelocationid];


ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_active] DEFAULT (1) FOR [active];


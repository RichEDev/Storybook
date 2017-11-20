ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_archived] DEFAULT (0) FOR [archived];


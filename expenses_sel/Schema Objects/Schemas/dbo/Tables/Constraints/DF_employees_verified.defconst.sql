ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_verified] DEFAULT (1) FOR [verified];


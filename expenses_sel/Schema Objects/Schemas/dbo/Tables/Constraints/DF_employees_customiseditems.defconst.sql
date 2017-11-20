ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_customiseditems] DEFAULT (0) FOR [customiseditems];


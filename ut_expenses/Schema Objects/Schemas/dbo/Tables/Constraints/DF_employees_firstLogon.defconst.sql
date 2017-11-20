ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_firstLogon] DEFAULT 0 FOR [firstLogon];


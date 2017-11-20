ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_currefnum] DEFAULT 0 FOR [currefnum];


ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_mileagetotal] DEFAULT (0) FOR [mileagetotal];


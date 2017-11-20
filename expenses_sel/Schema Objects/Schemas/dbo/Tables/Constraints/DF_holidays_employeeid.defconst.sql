ALTER TABLE [dbo].[holidays]
    ADD CONSTRAINT [DF_holidays_employeeid] DEFAULT (0) FOR [employeeid];


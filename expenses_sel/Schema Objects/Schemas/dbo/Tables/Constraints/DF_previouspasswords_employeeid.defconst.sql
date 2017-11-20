ALTER TABLE [dbo].[previouspasswords]
    ADD CONSTRAINT [DF_previouspasswords_employeeid] DEFAULT (0) FOR [employeeid];


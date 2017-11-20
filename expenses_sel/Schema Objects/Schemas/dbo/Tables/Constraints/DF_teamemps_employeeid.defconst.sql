ALTER TABLE [dbo].[teamemps]
    ADD CONSTRAINT [DF_teamemps_employeeid] DEFAULT (0) FOR [employeeid];


ALTER TABLE [dbo].[additems]
    ADD CONSTRAINT [DF_additems_employeeid] DEFAULT ((0)) FOR [employeeid];


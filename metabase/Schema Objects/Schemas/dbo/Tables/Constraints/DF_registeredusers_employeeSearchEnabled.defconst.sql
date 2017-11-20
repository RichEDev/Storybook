ALTER TABLE [dbo].[registeredusers]
    ADD CONSTRAINT [DF_registeredusers_employeeSearchEnabled] DEFAULT ((1)) FOR [employeeSearchEnabled];


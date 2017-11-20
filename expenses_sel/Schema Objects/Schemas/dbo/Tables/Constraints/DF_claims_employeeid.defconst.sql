ALTER TABLE [dbo].[claims_base]
    ADD CONSTRAINT [DF_claims_employeeid] DEFAULT ((0)) FOR [employeeid];


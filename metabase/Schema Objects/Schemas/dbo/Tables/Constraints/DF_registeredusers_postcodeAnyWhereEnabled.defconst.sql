ALTER TABLE [dbo].[registeredusers]
    ADD CONSTRAINT [DF_registeredusers_postcodeAnyWhereEnabled] DEFAULT ((1)) FOR [postcodeAnyWhereEnabled];


ALTER TABLE [dbo].[licenceRenewalTypes]
    ADD CONSTRAINT [DF_licenceRenewalTypes_archived] DEFAULT ((0)) FOR [archived];


ALTER TABLE [dbo].[codes_licencetypes]
    ADD CONSTRAINT [DF_codes_licencetypes_archived] DEFAULT ((0)) FOR [archived];


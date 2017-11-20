ALTER TABLE [dbo].[codes_contractstatus]
    ADD CONSTRAINT [DF_codes_contractstatus_isarchive] DEFAULT ((0)) FOR [isArchive];


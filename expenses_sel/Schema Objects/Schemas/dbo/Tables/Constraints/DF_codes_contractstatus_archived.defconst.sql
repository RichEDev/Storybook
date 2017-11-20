ALTER TABLE [dbo].[codes_contractstatus]
    ADD CONSTRAINT [DF_codes_contractstatus_archived] DEFAULT ((0)) FOR [archived];


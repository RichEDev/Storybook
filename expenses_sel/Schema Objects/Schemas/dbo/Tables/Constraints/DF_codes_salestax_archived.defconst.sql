ALTER TABLE [dbo].[codes_salestax]
    ADD CONSTRAINT [DF_codes_salestax_archived] DEFAULT ((0)) FOR [archived];


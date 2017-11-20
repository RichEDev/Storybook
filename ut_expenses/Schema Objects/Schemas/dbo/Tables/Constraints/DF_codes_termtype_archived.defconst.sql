ALTER TABLE [dbo].[codes_termtype]
    ADD CONSTRAINT [DF_codes_termtype_archived] DEFAULT ((0)) FOR [archived];


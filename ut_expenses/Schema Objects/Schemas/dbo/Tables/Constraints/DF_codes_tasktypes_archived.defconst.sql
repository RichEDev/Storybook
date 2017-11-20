ALTER TABLE [dbo].[codes_tasktypes]
    ADD CONSTRAINT [DF_codes_tasktypes_archived] DEFAULT ((0)) FOR [archived];


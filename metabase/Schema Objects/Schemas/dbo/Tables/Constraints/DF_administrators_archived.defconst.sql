ALTER TABLE [dbo].[administrators]
    ADD CONSTRAINT [DF_administrators_archived] DEFAULT ((0)) FOR [archived];


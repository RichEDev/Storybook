ALTER TABLE [dbo].[notes]
    ADD CONSTRAINT [DF_notes_read] DEFAULT ((0)) FOR [read];


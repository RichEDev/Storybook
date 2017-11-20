ALTER TABLE [dbo].[notes]
    ADD CONSTRAINT [DF_notes_datestamp] DEFAULT (getdate()) FOR [datestamp];


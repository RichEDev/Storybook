ALTER TABLE [dbo].[joinbreakdown_base]
    ADD CONSTRAINT [DF_joinbreakdown_amendedon] DEFAULT (getdate()) FOR [amendedon];


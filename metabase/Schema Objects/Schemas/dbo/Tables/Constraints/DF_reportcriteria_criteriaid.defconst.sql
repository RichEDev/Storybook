ALTER TABLE [dbo].[reportcriteria]
    ADD CONSTRAINT [DF_reportcriteria_criteriaid] DEFAULT (newid()) FOR [criteriaid];


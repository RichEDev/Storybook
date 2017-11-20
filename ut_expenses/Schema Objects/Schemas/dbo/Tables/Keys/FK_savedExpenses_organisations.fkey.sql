ALTER TABLE [dbo].[savedexpenses]
    ADD CONSTRAINT [FK_savedExpenses_organisations] FOREIGN KEY ([organisationIdentifier]) REFERENCES [dbo].[organisations] ([organisationID]);
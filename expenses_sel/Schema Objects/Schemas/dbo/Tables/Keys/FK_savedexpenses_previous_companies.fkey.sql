ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [FK_savedexpenses_previous_companies] FOREIGN KEY ([companyid]) REFERENCES [dbo].[companies] ([companyid]) ON DELETE NO ACTION ON UPDATE NO ACTION;


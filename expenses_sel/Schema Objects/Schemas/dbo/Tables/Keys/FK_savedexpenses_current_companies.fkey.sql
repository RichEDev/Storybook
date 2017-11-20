ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [FK_savedexpenses_current_companies] FOREIGN KEY ([companyid]) REFERENCES [dbo].[companies] ([companyid]) ON DELETE NO ACTION ON UPDATE NO ACTION;


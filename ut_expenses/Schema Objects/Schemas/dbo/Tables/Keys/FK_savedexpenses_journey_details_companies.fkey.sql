ALTER TABLE [dbo].[savedexpenses_journey_steps]
    ADD CONSTRAINT [FK_savedexpenses_journey_details_companies] FOREIGN KEY ([start_location]) REFERENCES [dbo].[companies] ([companyid]) ON DELETE NO ACTION ON UPDATE NO ACTION;


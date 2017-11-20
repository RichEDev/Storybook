ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [FK_contract_details_codes_termtype] FOREIGN KEY ([termTypeId]) REFERENCES [dbo].[codes_termtype] ([termTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


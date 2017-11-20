ALTER TABLE [dbo].[link_matrix]
    ADD CONSTRAINT [FK_link_matrix_contract_details] FOREIGN KEY ([contractId]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


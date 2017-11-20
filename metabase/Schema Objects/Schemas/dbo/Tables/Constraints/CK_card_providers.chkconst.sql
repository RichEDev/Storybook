ALTER TABLE [dbo].[card_providers]
    ADD CONSTRAINT [CK_card_providers] CHECK NOT FOR REPLICATION ([card_type]=(1) OR [card_type]=(2));


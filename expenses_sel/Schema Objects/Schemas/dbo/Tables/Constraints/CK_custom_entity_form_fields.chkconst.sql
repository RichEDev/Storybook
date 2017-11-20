ALTER TABLE [dbo].[custom_entity_form_fields]
    ADD CONSTRAINT [CK_custom_entity_form_fields] CHECK ([column]=(0) OR [column]=(1));


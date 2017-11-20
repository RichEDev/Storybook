ALTER TABLE [dbo].[customEntityFormFields]
    ADD CONSTRAINT [CK_customEntityFormFields] CHECK ([column]=(0) OR [column]=(1));


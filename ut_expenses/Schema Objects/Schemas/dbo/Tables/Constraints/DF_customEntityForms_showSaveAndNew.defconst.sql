ALTER TABLE [dbo].[customEntityForms]
    ADD CONSTRAINT [DF_customEntityForms_showSaveAndNew] DEFAULT ((1)) FOR [showSaveAndNew];


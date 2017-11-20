ALTER TABLE [dbo].[customEntityForms]
    ADD CONSTRAINT [DF_customEntityForms_showSaveAndStay] DEFAULT ((1)) FOR [showSaveAndStay];


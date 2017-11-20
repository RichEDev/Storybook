ALTER TABLE [dbo].[customEntityForms]
    ADD CONSTRAINT [DF_customEntityForms_showCancel] DEFAULT ((1)) FOR [showCancel];


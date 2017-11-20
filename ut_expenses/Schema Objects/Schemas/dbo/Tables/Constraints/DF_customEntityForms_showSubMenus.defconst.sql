ALTER TABLE [dbo].[customEntityForms]
    ADD CONSTRAINT [DF_customEntityForms_showSubMenus] DEFAULT ((1)) FOR [showSubMenus];


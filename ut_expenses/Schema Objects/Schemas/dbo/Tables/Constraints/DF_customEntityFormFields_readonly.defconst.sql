ALTER TABLE [dbo].[customEntityFormFields]
    ADD CONSTRAINT [DF_customEntityFormFields_readonly] DEFAULT ((0)) FOR [readonly];


ALTER TABLE [dbo].[globalESRElementFields]
    ADD CONSTRAINT [DF_globalESRElementFields_isMandatory] DEFAULT ((0)) FOR [isMandatory];


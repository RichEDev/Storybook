ALTER TABLE [dbo].[globalESRElementFields]
    ADD CONSTRAINT [DF_globalESRElementFields_isControlColumn] DEFAULT ((0)) FOR [isControlColumn];


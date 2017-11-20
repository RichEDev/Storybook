ALTER TABLE [dbo].[elementsBase]
    ADD CONSTRAINT [FK_elements_base_elements_base] FOREIGN KEY ([categoryID]) REFERENCES [dbo].[elementCategoryBase] ([categoryID]) ON DELETE CASCADE ON UPDATE CASCADE;


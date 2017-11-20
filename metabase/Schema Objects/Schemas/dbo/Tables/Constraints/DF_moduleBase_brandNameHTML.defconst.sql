ALTER TABLE [dbo].[moduleBase]
    ADD CONSTRAINT [DF_moduleBase_brandNameHTML] DEFAULT (N'<strong>Brand Name</strong>') FOR [brandNameHTML];


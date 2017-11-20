ALTER TABLE [dbo].[version_history]
    ADD CONSTRAINT [DF_Version_History_PlusMinusQty] DEFAULT ((0)) FOR [PlusMinusQty];


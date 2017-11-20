ALTER TABLE [dbo].[workflows]
    ADD CONSTRAINT [DF_Table_1_runOnChange] DEFAULT ((0)) FOR [runOnChange];


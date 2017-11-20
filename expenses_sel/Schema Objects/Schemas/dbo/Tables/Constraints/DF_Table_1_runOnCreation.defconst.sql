ALTER TABLE [dbo].[workflows]
    ADD CONSTRAINT [DF_Table_1_runOnCreation] DEFAULT ((0)) FOR [runOnCreation];


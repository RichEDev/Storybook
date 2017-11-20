ALTER TABLE [dbo].[accountProperties]
    ADD CONSTRAINT [DF_accountProperties_isGlobal] DEFAULT ((0)) FOR [isGlobal];


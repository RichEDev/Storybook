ALTER TABLE [dbo].[previouspasswords]
    ADD CONSTRAINT [DF_previouspasswords_passwordMethod] DEFAULT ((4)) FOR [passwordMethod];


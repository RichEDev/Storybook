ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [DF_savedexpenses_current_bmiles] DEFAULT ((0)) FOR [bmiles];


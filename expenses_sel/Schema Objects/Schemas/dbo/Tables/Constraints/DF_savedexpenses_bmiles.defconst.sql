ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [DF_savedexpenses_bmiles] DEFAULT ((0)) FOR [bmiles];


ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [DF_savedexpenses_current_returned] DEFAULT ((0)) FOR [returned];


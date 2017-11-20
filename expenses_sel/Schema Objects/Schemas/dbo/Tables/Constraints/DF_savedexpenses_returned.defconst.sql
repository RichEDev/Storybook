ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [DF_savedexpenses_returned] DEFAULT (0) FOR [returned];


ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [DF_savedexpenses_receipt] DEFAULT (0) FOR [receipt];


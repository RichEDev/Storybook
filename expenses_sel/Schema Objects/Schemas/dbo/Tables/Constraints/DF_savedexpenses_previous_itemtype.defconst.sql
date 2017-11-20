ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [DF_savedexpenses_previous_itemtype] DEFAULT (1) FOR [itemtype];


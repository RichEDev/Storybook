ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [DF_savedexpenses_current_previous_itemtype] DEFAULT ((1)) FOR [itemtype];


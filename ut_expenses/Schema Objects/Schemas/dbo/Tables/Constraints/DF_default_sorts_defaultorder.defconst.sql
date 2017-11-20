ALTER TABLE [dbo].[default_sorts]
    ADD CONSTRAINT [DF_default_sorts_defaultorder] DEFAULT (0) FOR [defaultorder];


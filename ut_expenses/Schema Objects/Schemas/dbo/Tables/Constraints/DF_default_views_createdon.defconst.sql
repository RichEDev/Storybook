ALTER TABLE [dbo].[default_views]
    ADD CONSTRAINT [DF_default_views_createdon] DEFAULT (getdate()) FOR [createdon];


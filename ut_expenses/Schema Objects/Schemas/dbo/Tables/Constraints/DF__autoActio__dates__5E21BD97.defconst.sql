ALTER TABLE [dbo].[autoActionLog]
    ADD CONSTRAINT [DF__autoActio__dates__5E21BD97] DEFAULT (getdate()) FOR [datestamp];


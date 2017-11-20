ALTER TABLE [dbo].[userdefinedGroupingAssociation]
    ADD CONSTRAINT [FK_userdefinedGroupingAssociation_userdefinedGroupings] FOREIGN KEY ([groupingId]) REFERENCES [dbo].[userdefinedGroupings] ([userdefinedGroupID]) ON DELETE CASCADE ON UPDATE NO ACTION;


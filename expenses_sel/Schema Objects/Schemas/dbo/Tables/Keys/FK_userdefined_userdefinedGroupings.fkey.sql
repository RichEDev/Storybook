ALTER TABLE [dbo].[userdefined]
    ADD CONSTRAINT [FK_userdefined_userdefinedGroupings] FOREIGN KEY ([groupID]) REFERENCES [dbo].[userdefinedGroupings] ([userdefinedGroupID]) ON DELETE SET NULL ON UPDATE NO ACTION;


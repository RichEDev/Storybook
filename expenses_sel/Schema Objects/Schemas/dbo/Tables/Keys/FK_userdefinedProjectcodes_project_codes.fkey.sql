ALTER TABLE [dbo].[userdefinedProjectcodes]
    ADD CONSTRAINT [FK_userdefinedProjectcodes_project_codes] FOREIGN KEY ([projectcodeid]) REFERENCES [dbo].[project_codes] ([projectcodeid]) ON DELETE CASCADE ON UPDATE NO ACTION;


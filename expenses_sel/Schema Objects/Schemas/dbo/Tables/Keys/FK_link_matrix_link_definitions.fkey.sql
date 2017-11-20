ALTER TABLE [dbo].[link_matrix]
    ADD CONSTRAINT [FK_link_matrix_link_definitions] FOREIGN KEY ([linkId]) REFERENCES [dbo].[link_definitions] ([linkId]) ON DELETE CASCADE ON UPDATE NO ACTION;


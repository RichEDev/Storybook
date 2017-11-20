ALTER TABLE [dbo].[userdefined_costcodes]
    ADD CONSTRAINT [FK_userdefined_costcodes_costcodes] FOREIGN KEY ([costcodeid]) REFERENCES [dbo].[costcodes] ([costcodeid]) ON DELETE CASCADE ON UPDATE NO ACTION;


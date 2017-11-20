ALTER TABLE [dbo].[savedexpenses_costcodes]
    ADD CONSTRAINT [FK_savedexpenses-costcodes_costcodes] FOREIGN KEY ([costcodeid]) REFERENCES [dbo].[costcodes] ([costcodeid]) ON DELETE NO ACTION ON UPDATE CASCADE;


ALTER TABLE [dbo].[savedexpenses_costcodes]
    ADD CONSTRAINT [FK_savedexpenses-costcodes_departments] FOREIGN KEY ([departmentid]) REFERENCES [dbo].[departments] ([departmentid]) ON DELETE NO ACTION ON UPDATE NO ACTION;


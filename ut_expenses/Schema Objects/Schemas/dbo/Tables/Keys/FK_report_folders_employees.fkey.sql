ALTER TABLE [dbo].[report_folders]
    ADD CONSTRAINT [FK_report_folders_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;


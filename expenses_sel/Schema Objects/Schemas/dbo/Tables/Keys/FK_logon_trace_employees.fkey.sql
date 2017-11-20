ALTER TABLE [dbo].[logon_trace]
    ADD CONSTRAINT [FK_logon_trace_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;


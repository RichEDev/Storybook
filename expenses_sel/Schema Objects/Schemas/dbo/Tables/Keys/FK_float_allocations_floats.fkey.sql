ALTER TABLE [dbo].[float_allocations]
    ADD CONSTRAINT [FK_float_allocations_floats] FOREIGN KEY ([floatid]) REFERENCES [dbo].[floats] ([floatid]) ON DELETE NO ACTION ON UPDATE NO ACTION;


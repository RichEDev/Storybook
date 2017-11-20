ALTER TABLE [dbo].[employee_roles]
    ADD CONSTRAINT [FK_employee_roles_item_roles] FOREIGN KEY ([itemroleid]) REFERENCES [dbo].[item_roles] ([itemroleid]) ON DELETE CASCADE ON UPDATE NO ACTION;


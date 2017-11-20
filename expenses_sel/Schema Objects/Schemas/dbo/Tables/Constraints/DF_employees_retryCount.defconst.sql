ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_retryCount] DEFAULT ((0)) FOR [retryCount];


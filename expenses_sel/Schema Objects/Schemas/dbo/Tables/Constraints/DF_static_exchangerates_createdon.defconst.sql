ALTER TABLE [dbo].[static_exchangerates]
    ADD CONSTRAINT [DF_static_exchangerates_createdon] DEFAULT (getdate()) FOR [createdon];

